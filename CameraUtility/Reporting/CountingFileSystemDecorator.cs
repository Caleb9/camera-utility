using System;
using System.Collections.Generic;
using CameraUtility.FileSystemIsolation;
using JetBrains.Annotations;

namespace CameraUtility.Reporting
{
    internal sealed class CountingFileSystemDecorator
        : IFileSystem
    {
        [NotNull] private readonly IFileSystem _decorated;
        [NotNull] private readonly Report _report;

        public CountingFileSystemDecorator(
            [NotNull] IFileSystem decorated,
            Report report)
        {
            _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
            _report = report;
        }

        IEnumerable<string> IFileSystem.GetFiles(string directory, string searchMask)
        {
            return _decorated.GetFiles(directory, searchMask);
        }

        bool IFileSystem.CreateDirectoryIfNotExists(string path)
        {
            return _decorated.CreateDirectoryIfNotExists(path);
        }

        bool IFileSystem.CopyFileIfDoesNotExist(string source, string destination)
        {
            var result = _decorated.CopyFileIfDoesNotExist(source, destination);
            if (result)
            {
                _report.IncrementNumberOfFilesCopied();
            }
            else
            {
                _report.AddSkippedFile(source, destination);
            }

            return result;
        }

        string IFileSystem.CombinePaths(params string[] paths)
        {
            return _decorated.CombinePaths(paths);
        }

        bool IFileSystem.Exists(string path)
        {
            return _decorated.Exists(path);
        }
    }
}