using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace CameraUtility.FileSystemIsolation
{
    internal sealed class FileSystem : IFileSystem
    {
        bool IFileSystem.CreateDirectoryIfNotExists(
            string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            }

            var exists = Directory.Exists(path);
            if (!exists)
            {
                Directory.CreateDirectory(path);
            }

            Debug.WriteLineIf(!exists, $"Created {path}");
            return exists;
        }

        bool IFileSystem.CopyFileIfDoesNotExist(
            string source,
            string destination)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(source));
            }

            if (string.IsNullOrWhiteSpace(destination))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(destination));
            }

            var exists = File.Exists(destination);
            if (!exists)
            {
                File.Copy(source, destination, false);
            }
            Debug.WriteLine((exists ? "Skipped": "Copied ") + $" {source} -> {destination}");

            return !exists;
        }

        IEnumerable<string> IFileSystem.GetFiles(
            string directory,
            string searchMask)
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(directory));
            }

            if (string.IsNullOrWhiteSpace(searchMask))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(searchMask));
            }

            return Directory.GetFiles(
                directory,
                searchMask,
                SearchOption.AllDirectories);
        }

        string IFileSystem.CombinePaths(
            params string[] paths)
        {
            return Path.Combine(paths);
        }

        bool IFileSystem.Exists(string path)
        {
            return File.Exists(path) || Directory.Exists(path);
        }
    }
}