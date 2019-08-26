using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CanonEosPhotoDownloader.FileSystemIsolation;
using JetBrains.Annotations;

namespace CanonEosPhotoDownloader
{
    internal sealed class CameraFileFinder : ICameraFileFinder
    {
        [NotNull]
        private static readonly string[] ImageFileExtensions =
        {
            ".jpg",
            ".cr2",
            ".dng",
            ".mp4"
        };

        [NotNull] private readonly IFileSystem _fileSystem;

        internal CameraFileFinder(
            [NotNull] IFileSystem fileSystem)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        [NotNull]
        [ItemNotNull]
        IEnumerable<string> ICameraFileFinder.FindCameraFiles(
            [NotNull] string directory)
        {
            return FindFilePaths(directory).Where(IsCameraFile).AsParallel();
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<string> FindFilePaths(
            [NotNull] string directory)
        {
            try
            {
                return _fileSystem.GetFiles(directory);
            }
            catch (DirectoryNotFoundException exception)
            {
                /* Wrapped to customize the message. Invalid directory is often obfuscated in the original exception
                 * because Windows tries to prepend it with user's profile directory. We want to output the original
                 * directory as entered by the user. */
                throw new DirectoryNotFoundException($"{directory} directory not found", exception);
            }
        }

        private bool IsCameraFile(
            [NotNull] string filePath)
        {
            const bool ignoreCase = true;
            return ImageFileExtensions.Any(
                extension => filePath.EndsWith(extension, ignoreCase, CultureInfo.InvariantCulture));
        }
    }
}