using System;
using System.Diagnostics;
using System.IO;
using CanonEosPhotoDownloader.FileSystemIsolation;
using JetBrains.Annotations;

namespace CanonEosPhotoDownloader
{
    public sealed class CameraImageCopier
    {
        [NotNull] private readonly ICameraFileFinder _cameraFileFinder;
        [NotNull] private readonly ICameraFileNameConverter _cameraFileNameConverter;
        [NotNull] private readonly IFileSystem _fileSystem;

        public CameraImageCopier(
            [NotNull] IFileSystem fileSystem,
            [NotNull] ICameraFileFinder cameraFileFinder,
            [NotNull] ICameraFileNameConverter cameraFileNameConverter)
        {
            _fileSystem =
                fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _cameraFileFinder = cameraFileFinder ?? throw new ArgumentNullException(nameof(cameraFileFinder));
            _cameraFileNameConverter = cameraFileNameConverter ??
                                       throw new ArgumentNullException(nameof(cameraFileNameConverter));
        }

        [CanBeNull] internal TextWriter Console { private get; set; } = TextWriter.Null;

        public void CopyFiles(
            [NotNull] string sourceDirectory,
            [NotNull] string destinationDirectoryRoot,
            bool dryRun = false)
        {
            if (string.IsNullOrWhiteSpace(sourceDirectory))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(sourceDirectory));
            }

            if (string.IsNullOrWhiteSpace(destinationDirectoryRoot))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(destinationDirectoryRoot));
            }

            foreach (var cameraFilePath in _cameraFileFinder.FindFilePaths(sourceDirectory))
            {
                CopyFile(cameraFilePath, destinationDirectoryRoot, dryRun);
            }
        }

        private void CopyFile(
            string cameraFilePath,
            string destinationDirectoryRoot,
            bool dryRun)
        {
            var (destinationDirectory, destinationFileFullName) =
                _cameraFileNameConverter.Convert(cameraFilePath, destinationDirectoryRoot);

            Debug.WriteLine($"{cameraFilePath} -> {destinationFileFullName}");

            if (!dryRun)
            {
                ExecuteCopyFile(cameraFilePath, destinationDirectory, destinationFileFullName);
            }
            else
            {
                PretendCopyFile(cameraFilePath, destinationFileFullName);
            }
        }

        private void ExecuteCopyFile(
            string cameraFilePath,
            string destinationPath,
            string destinationFileFullName)
        {
            if (_fileSystem.CreateDirectoryIfNotExists(destinationPath))
            {
                Console?.WriteLine($"Created {destinationPath}");
            }

            Console?.WriteLine(
                _fileSystem.FileCopyIfDoesNotExist(cameraFilePath, destinationFileFullName)
                    ? $"{cameraFilePath} -> {destinationFileFullName}"
                    : $"Skipped {cameraFilePath} ({destinationFileFullName} already exists).");
        }

        private void PretendCopyFile(
            string cameraFilePath,
            string destinationFileFullName)
        {
            Console?.WriteLine(
                !_fileSystem.Exists(destinationFileFullName)
                    ? $"{cameraFilePath} -> {destinationFileFullName}"
                    : $"Skip {cameraFilePath} ({destinationFileFullName} already exists).");
        }
    }
}