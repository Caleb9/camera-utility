using CameraUtility.CameraFiles;
using CameraUtility.Commands.Check.Output;
using CameraUtility.Exif;

namespace CameraUtility.Commands.Check.Execution;

internal sealed class FileChecker
{
    private const int NoErrorsResultCode = 0;
    private const int ErrorResultCode = 3;
    private readonly CameraFileFactory _cameraFileFactory;

    private readonly CameraFilesFinder _cameraFilesFinder;

    private readonly List<CameraFilePath> _cameraFilesWithoutMetadata = new();
    private readonly ConsoleOutput _consoleOutput;
    private readonly IMetadataReader _metadataReader;

    internal FileChecker(
        CameraFilesFinder cameraFilesFinder,
        IMetadataReader metadataReader,
        CameraFileFactory cameraFileFactory,
        ConsoleOutput consoleOutput)
    {
        _cameraFilesFinder = cameraFilesFinder;
        _metadataReader = metadataReader;
        _cameraFileFactory = cameraFileFactory;
        _consoleOutput = consoleOutput;
    }

    internal int Execute(CheckCommand.OptionArgs args)
    {
        long filesCounter = 0;
        foreach (var sourcePath in args.SourcePaths.Value)
        {
            var cameraFilesResult = _cameraFilesFinder.FindCameraFiles(sourcePath);
            if (cameraFilesResult.IsFailure)
            {
                _consoleOutput.PrintError(cameraFilesResult.Error);
                return ErrorResultCode;
            }

            filesCounter += cameraFilesResult.Value.Count();
            foreach (var cameraFilePath in cameraFilesResult.Value)
            {
                var exifTags = _metadataReader.ExtractTags(cameraFilePath);
                var cameraFileResult = _cameraFileFactory.Create(cameraFilePath, exifTags);
                if (cameraFileResult.IsFailure)
                {
                    _cameraFilesWithoutMetadata.Add(cameraFilePath);
                }
            }
        }

        _consoleOutput.PrintSummary(
            filesCounter,
            _cameraFilesWithoutMetadata);

        return NoErrorsResultCode;
    }
}