using System.IO;
using CameraUtility.FileSystemIsolation;
using JetBrains.Annotations;

namespace CameraUtility
{
    public sealed class CameraFileCopier : ICameraFileCopier
    {
        [NotNull] private readonly ICameraFileNameConverter _cameraFileNameConverter;
        [NotNull] private readonly IFileSystem _fileSystem;

        public CameraFileCopier(
            [NotNull] ICameraFileNameConverter cameraFileNameConverter,
            [NotNull] IFileSystem fileSystem)
        {
            _cameraFileNameConverter = cameraFileNameConverter;
            _fileSystem = fileSystem;
        }

        /// <summary>
        ///     TextWriter to write output of the program to. By default it is TextWriter.Null so no output is
        ///     generated (useful in tests), but <see cref="Program" /> sets it to System.IO.Console.Out so output is
        ///     printed to the console.
        /// </summary>
        [CanBeNull]
        internal TextWriter Console { private get; set; } = TextWriter.Null;

        void ICameraFileCopier.ExecuteCopyFile(
            string cameraFilePath,
            string destinationDirectoryRoot)
        {
            var (destinationPath, destinationFileFullName) =
                _cameraFileNameConverter.Convert(cameraFilePath, destinationDirectoryRoot);

            if (_fileSystem.CreateDirectoryIfNotExists(destinationPath))
            {
                Console?.WriteLine($"Created {destinationPath}");
            }

            Console?.WriteLine(
                _fileSystem.CopyFileIfDoesNotExist(cameraFilePath, destinationFileFullName)
                    ? $"{cameraFilePath} -> {destinationFileFullName}"
                    : $"Skipped {cameraFilePath} ({destinationFileFullName} already exists).");
        }

        void ICameraFileCopier.PretendCopyFile(
            string cameraFilePath,
            string destinationDirectoryRoot)
        {
            var (_, destinationFileFullName) =
                _cameraFileNameConverter.Convert(cameraFilePath, destinationDirectoryRoot);

            Console?.WriteLine(
                !_fileSystem.Exists(destinationFileFullName)
                    ? $"{cameraFilePath} -> {destinationFileFullName}"
                    : $"Skip {cameraFilePath} ({destinationFileFullName} already exists).");
        }
    }
}