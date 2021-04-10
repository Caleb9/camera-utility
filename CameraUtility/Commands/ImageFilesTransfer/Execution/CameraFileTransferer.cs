using System;
using System.IO.Abstractions;
using CameraUtility.Commands.ImageFilesTransfer.Options;
using CSharpFunctionalExtensions;

namespace CameraUtility.Commands.ImageFilesTransfer.Execution
{
    internal sealed class CameraFileTransferer
    {
        private readonly CameraFileNameConverter _cameraFileNameConverter;
        private readonly IFileSystem _fileSystem;
        private readonly TransferFiles _transferFiles;

        internal CameraFileTransferer(
            CameraFileNameConverter cameraFileNameConverter,
            IFileSystem fileSystem,
            TransferFiles transferFiles)
        {
            _cameraFileNameConverter = cameraFileNameConverter;
            _fileSystem = fileSystem;
            _transferFiles = transferFiles;
        }

        internal Result TransferFile(Args args)
        {
            var convertResult = _cameraFileNameConverter.Convert(args);
            if (convertResult.IsFailure)
            {
                return convertResult;
            }

            var (destinationDirectory, destinationFileName) = convertResult.Value;
            if (!_fileSystem.Directory.Exists(destinationDirectory) && !args.DryRun)
            {
                _fileSystem.Directory.CreateDirectory(destinationDirectory);
                OnDirectoryCreated(this, destinationDirectory);
            }

            var destinationFilePath =
                new CameraFilePath(_fileSystem.Path.Combine(destinationDirectory, destinationFileName));
            var destinationAlreadyExists = _fileSystem.File.Exists(destinationFilePath);
            if (destinationAlreadyExists && !args.Overwrite)
            {
                OnFileSkipped(this, (args.CameraFilePath, destinationFilePath));
                return Result.Success();
            }

            if (!args.DryRun)
            {
                _transferFiles(args.CameraFilePath, destinationFilePath, args.Overwrite);
            }

            OnFileTransferred(this, (args.CameraFilePath, destinationFilePath, args.DryRun));
            return Result.Success();
        }

        internal event EventHandler<string> OnDirectoryCreated =
            (_, _) => { };

        internal event EventHandler<(CameraFilePath sourceFile, CameraFilePath destinationFile)> OnFileSkipped =
            (_, _) => { };

        internal event EventHandler<(CameraFilePath sourceFile, CameraFilePath destinationFile, DryRun dryRun)>
            OnFileTransferred = (_, _) => { };

        internal delegate void TransferFiles(
            CameraFilePath sourcePath,
            CameraFilePath destinationPath,
            bool overwrite);

        internal sealed record Args(
            CameraFilePath CameraFilePath,
            DestinationDirectory DestinationRootDirectory,
            DryRun DryRun,
            SkipDateSubdirectory SkipDateSubdirectory,
            Overwrite Overwrite);
    }
}