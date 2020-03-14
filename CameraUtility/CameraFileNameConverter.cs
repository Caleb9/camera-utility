using System;
using CameraUtility.CameraFiles;
using CameraUtility.Exif;
using CameraUtility.FileSystemIsolation;

namespace CameraUtility
{
    public sealed class CameraFileNameConverter
        : ICameraFileNameConverter
    {
        private readonly ICameraFileFactory _cameraFileFactory;
        private readonly IFileSystem _fileSystem;
        private readonly IMetadataReader _metadataReader;

        public CameraFileNameConverter(
            IMetadataReader metadataReader,
            ICameraFileFactory cameraFileFactory,
            IFileSystem fileSystem)
        {
            _metadataReader = metadataReader;
            _fileSystem = fileSystem;
            _cameraFileFactory = cameraFileFactory;
        }

        public bool SkipDateSubDirectory { get; set; } = false;

        (string destinationDirectory, string destinationFileFullName) ICameraFileNameConverter.Convert(
            string cameraFilePath,
            string destinationRootPath)
        {
            var cameraFile = GetCameraFile(cameraFilePath);
            var destinationDirectory = GetDestinationDirectory(destinationRootPath, cameraFile);
            var destinationFileFullName = GetDestinationFileFullName(destinationDirectory, cameraFile);

            return (destinationDirectory, destinationFileFullName);
        }

        private ICameraFile GetCameraFile(
            string cameraFilePath)
        {
            var metadataTags = _metadataReader.ExtractTags(cameraFilePath);
            return _cameraFileFactory.Create(cameraFilePath, metadataTags);
        }

        private string GetDestinationDirectory(
            string destinationRootPath,
            ICameraFile cameraFile)
        {
            if (SkipDateSubDirectory)
            {
                return destinationRootPath;
            }
            var destinationSubDirectory = GetDateSubDirectoryName(cameraFile.Created);
            return _fileSystem.CombinePaths(destinationRootPath, destinationSubDirectory);
        }

        private string GetDateSubDirectoryName(
            DateTime created)
        {
            return $"{created.Year:0000}_{created.Month:00}_{created.Day:00}";
        }

        private string GetDestinationFileFullName(
            string destinationDirectory, 
            ICameraFile cameraFile)
        {
            var fileName = NewCameraFileName(cameraFile);
            var destinationFileFullName = _fileSystem.CombinePaths(destinationDirectory, fileName);
            return destinationFileFullName;
        }

        private string NewCameraFileName(
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
}