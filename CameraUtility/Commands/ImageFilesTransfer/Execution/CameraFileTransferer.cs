using System;
using System.IO.Abstractions;
using CameraUtility.Commands.ImageFilesTransfer.Options;

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

        internal void TransferFile(Args args)
        {
            var (destinationDirectory, destinationFileName) = _cameraFileNameConverter.Convert(args);

            if (!_fileSystem.Directory.Exists(destinationDirectory) && !args.DryRun)
            {
                _fileSystem.Directory.CreateDirectory(destinationDirectory);
                OnDirectoryCreated(this, destinationDirectory);
            }

            var destinationFilePath = _fileSystem.Path.Combine(destinationDirectory, destinationFileName);
            var destinationAlreadyExists = _fileSystem.File.Exists(destinationFilePath);
            if (destinationAlreadyExists)
            {
                OnFileSkipped(this, (args.CameraFilePath, destinationFilePath));
                return;
            }

            if (!args.DryRun)
            {
                _transferFiles(args.CameraFilePath, destinationFilePath);
            }

            OnFileTransferred(this, (args.CameraFilePath, destinationFilePath, args.DryRun));
        }

        internal event EventHandler<string> OnDirectoryCreated =
            (_, _) => { };

        internal event EventHandler<(string sourceFile, string destinationFile)> OnFileSkipped =
            (_, _) => { };

        internal event EventHandler<(string sourceFile, string destinationFile, DryRun dryRun)> OnFileTransferred =
            (_, _) => { };

        internal delegate void TransferFiles(
            string sourcePath,
            string destinationPath,
            bool overwrite = false);

        internal sealed record Args(
            string CameraFilePath,
            DestinationDirectory DestinationRootDirectory,
            DryRun DryRun,
            SkipDateSubdirectory SkipDateSubdirectory);
    }
}