using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CanonEosPhotoDownloader.FileSystemIsolation;
using JetBrains.Annotations;

namespace CanonEosPhotoDownloader
{
    internal sealed class CameraFileFinder : ICameraFileFinder
    {
        [NotNull] private static readonly string[] ImageFileExtensions =
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
        IEnumerable<string> ICameraFileFinder.FindFilePaths(string directory)
        {
            return ImageFileExtensions.SelectMany(extension => FindFilePaths(directory, extension));
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<string> FindFilePaths([NotNull] string directory, string fileExtension)
        {
            try
            {
                return _fileSystem.GetFiles(
                    directory,
                    $"*{fileExtension}");
            }
            catch (DirectoryNotFoundException exception)
            {
                /* Wrapped to customize the message. Invalid directory is often obfuscated in the original exception
                 * because Windows tries to prepend it with user's profile directory. We want to output the original
                 * directory as entered by the user. */
                throw new DirectoryNotFoundException($"{directory} directory not found", exception);
            }
        }
    }
}