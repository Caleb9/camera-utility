using System.IO;
using CameraUtility.FileSystemIsolation;
using JetBrains.Annotations;

namespace CameraUtility
{
    public sealed class CameraFileCopier : ICameraFileCopier
    {
        [NotNull] private readonly ICameraFileNameConverter _cameraFileNameConverter;
        [NotNull] private readonly IFileSystem _fileSystem;
        private readonly bool _pretend;

        public CameraFileCopier(
            [NotNull] ICameraFileNameConverter cameraFileNameConverter,
            [NotNull] IFileSystem fileSystem,
            bool pretend)
        {
            _cameraFileNameConverter = cameraFileNameConverter;
            _fileSystem = fileSystem;
            _pretend = pretend;
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

            if (_fileSystem.CreateDirectoryIfNotExists(destinationPath, _pretend))
            {
                Console?.WriteLine($"Created {destinationPath}");
            }

            var copied = _fileSystem.CopyFileIfDoesNotExist(cameraFilePath, destinationFileFullName, _pretend);

            Console?.WriteLine(
                copied
                    ? $"{cameraFilePath} -> {destinationFileFullName}"
                    : $"Skipped {cameraFilePath} ({destinationFileFullName} already exists).");
        }
    }
}