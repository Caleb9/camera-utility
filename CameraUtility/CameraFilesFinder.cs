﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using CSharpFunctionalExtensions;

namespace CameraUtility
{
    internal sealed class CameraFilesFinder
    {
        private static readonly string[] CameraFileExtensions =
        {
            ".jpg",
            ".jpeg",
            ".cr2",
            ".dng",
            ".mp4",
            ".mov"
        };

        private readonly IFileSystem _fileSystem;

        internal CameraFilesFinder(
            IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        internal event EventHandler<long> OnCameraFilesFound = (_, _) => { };

        internal Result<IEnumerable<string>> FindCameraFiles(
            string path)
        {
            if (_fileSystem.Directory.Exists(path) is false && _fileSystem.File.Exists(path) is false)
            {
                return Result.Failure<IEnumerable<string>>($"{path} does not exist.");
            }

            var result = FindFilePaths(path).Where(IsCameraFile).AsParallel().ToList();
            OnCameraFilesFound(this, result.Count);
            return result;
        }

        private IEnumerable<string> FindFilePaths(
            string path)
        {
            return _fileSystem.Directory.Exists(path)
                ? _fileSystem.Directory.GetFiles(path, "*", SearchOption.AllDirectories)
                : new[] {path};
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