using System;
using System.Collections.Generic;
using CameraUtility.FileSystemIsolation;

namespace CameraUtility.Reporting
{
    internal sealed class CountingFileSystemDecorator
        : IFileSystem
    {
        private readonly IFileSystem _decorated;
        private readonly Report _report;

        public CountingFileSystemDecorator(
            IFileSystem decorated,
            Report report)
        {
            _decorated = decorated;
            _report = report;
        }

        IEnumerable<string> IFileSystem.GetFiles(string directory, string searchMask)
        {
            return _decorated.GetFiles(directory, searchMask);
        }

        bool IFileSystem.CreateDirectoryIfNotExists(
            string path,
            bool pretend)
        {
            return _decorated.CreateDirectoryIfNotExists(path, pretend);
        }

        bool IFileSystem.CopyFileIfDoesNotExist(
            string source,
            string destination,
            bool pretend)
        {
            var copied = _decorated.CopyFileIfDoesNotExist(source, destination, pretend);
            ProcessReport(copied, pretend, source, destination);
            return copied;
        }

        bool IFileSystem.MoveFileIfDoesNotExist(
            string source,
            string destination,
            bool pretend)
        {
            var moved = _decorated.MoveFileIfDoesNotExist(source, destination, pretend);
            ProcessReport(moved, pretend, source, destination);
            return moved;
        }

        private void ProcessReport(
            bool copied,
            bool pretend,
            string source,
            string destination)
        {
            if (copied)
            {
                if (!pretend)
                {
                    _report.IncrementNumberOfFilesCopied();
                }
            }
            else
            {
                _report.AddSkippedFile(source, destination);
            }
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