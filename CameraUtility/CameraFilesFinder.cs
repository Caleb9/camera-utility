using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using CameraUtility.FileSystemIsolation;

namespace CameraUtility
{
    public sealed class CameraFilesFinder : ICameraFilesFinder
    {
        private static readonly string[] CameraFileExtensions =
        {
            ".jpg",
            ".jpeg",
            ".cr2",
            ".dng",
            ".mp4"
        };

        private readonly IFileSystem _fileSystem;

        public CameraFilesFinder(
            IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }


        IEnumerable<string> ICameraFilesFinder.FindCameraFiles(
            string path)
        {
            if (!_fileSystem.Exists(path))
            {
                throw new PathNotFoundException($"{path} not found");
            }

            return FindFilePaths(path).Where(IsCameraFile).AsParallel();
        }


        private IEnumerable<string> FindFilePaths(
            string path)
        {
            Debug.Assert(_fileSystem.Exists(path));

            return _fileSystem.IsDirectory(path) ? _fileSystem.GetFiles(path) : new[] {path};
        }

        private bool IsCameraFile(
            string filePath)
        {
            const bool ignoreCase = true;
            return CameraFileExtensions.Any(
                supportedExtension => filePath.EndsWith(supportedExtension, ignoreCase, CultureInfo.InvariantCulture));
        }

        public sealed class PathNotFoundException : IOException
        {
            public PathNotFoundException(string? message, Exception? innerException = null)
                : base(message, innerException)
            {
            }
        }
    }
}