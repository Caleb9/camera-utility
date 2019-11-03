using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CameraUtility.FileSystemIsolation;

namespace CameraUtility
{
    internal sealed class CameraFilesFinder : ICameraFilesFinder
    {
        private static readonly string[] CameraFileExtensions =
        {
            ".jpg",
            ".cr2",
            ".dng",
            ".mp4"
        };

        private readonly IFileSystem _fileSystem;

        internal CameraFilesFinder(
            IFileSystem fileSystem)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }


        IEnumerable<string> ICameraFilesFinder.FindCameraFiles(
            string directory)
        {
            return FindFilePaths(directory).Where(IsCameraFile).AsParallel();
        }


        private IEnumerable<string> FindFilePaths(
            string directory)
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
            string filePath)
        {
            const bool ignoreCase = true;
            return CameraFileExtensions.Any(
                supportedExtension => filePath.EndsWith(supportedExtension, ignoreCase, CultureInfo.InvariantCulture));
        }
    }
}