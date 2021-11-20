using System.IO.Abstractions;
using CameraUtility.CameraFiles;
using CameraUtility.Commands.ImageFilesTransfer.Execution;
using CameraUtility.Exif;
using CSharpFunctionalExtensions;

namespace CameraUtility.Commands.ImageFilesTransfer;

internal sealed class CameraFileNameConverter
{
    private readonly CameraFileFactory _cameraFileFactory;
    private readonly IFileSystem _fileSystem;
    private readonly IMetadataReader _metadataReader;

    internal CameraFileNameConverter(
        IMetadataReader metadataReader,
        CameraFileFactory cameraFileFactory,
        IFileSystem fileSystem)
    {
        _metadataReader = metadataReader;
        _cameraFileFactory = cameraFileFactory;
        _fileSystem = fileSystem;
    }

    internal Result<(string destinationDirectory, string destinationFileName)> Convert(
        CameraFileTransferer.Args args)
    {
        var (cameraFilePath, destinationRootDirectory, _, skipDateSubdirectoryOption, _) = args;
        var cameraFileResult = GetCameraFile(cameraFilePath);
        if (cameraFileResult.IsFailure)
        {
            return Result.Failure<(string, string)>(cameraFileResult.Error);
        }

        var destinationDirectory =
            GetDestinationDirectory(destinationRootDirectory, cameraFileResult.Value, skipDateSubdirectoryOption);
        var destinationFileName = GetDestinationFileName(cameraFileResult.Value);

        return (destinationDirectory, destinationFileName);
    }

    private Result<ICameraFile> GetCameraFile(
        CameraFilePath cameraFilePath)
    {
        var metadataTags = _metadataReader.ExtractTags(cameraFilePath);
        return _cameraFileFactory.Create(cameraFilePath, metadataTags);
    }

    private string GetDestinationDirectory(
        string destinationRootPath,
        ICameraFile cameraFile,
        bool skipDateSubDirectory)
    {
        if (skipDateSubDirectory)
        {
            return destinationRootPath;
        }

        var destinationSubDirectory = GetDateSubDirectoryName(cameraFile.Created);
        return _fileSystem.Path.Combine(destinationRootPath, destinationSubDirectory);
    }

    private string GetDateSubDirectoryName(
        DateTime created)
    {
        return $"{created.Year:0000}_{created.Month:00}_{created.Day:00}";
    }

    private string GetDestinationFileName(
        ICameraFile cameraFile)
    {
        return $"{cameraFile.DestinationNamePrefix}{GetDateForFileName(cameraFile.Created)}" +
               $"_{GetTimeForFileName(cameraFile.Created)}{cameraFile.Extension}";
    }

    private string GetDateForFileName(
        DateTime created)
    {
        return $"{created.Year:0000}{created.Month:00}{created.Day:00}";
    }

    private string GetTimeForFileName(
        DateTime created)
    {
        return $"{created.Hour:00}{created.Minute:00}{created.Second:00}{created.Millisecond:000}";
    }
}