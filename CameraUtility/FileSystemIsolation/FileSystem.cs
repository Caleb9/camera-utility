﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace CameraUtility.FileSystemIsolation
{
    internal sealed class FileSystem : IFileSystem
    {
        bool IFileSystem.CreateDirectoryIfNotExists(
            string path,
            bool pretend)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            }

            var exists = Directory.Exists(path);
            if (exists)
            {
                return false;
            }

            if (!pretend)
            {
                Directory.CreateDirectory(path);
                Debug.WriteLine($"Created {path}");
            }

            return true;
        }

        bool IFileSystem.CopyFileIfDoesNotExist(
            string source,
            string destination,
            bool pretend)
        {
            var copied = CopyOrMove(source, destination, pretend, File.Copy);
            Debug.WriteLineIf(!pretend && copied, $"Copied {source} -> {destination}");
            return copied;
        }

        bool IFileSystem.MoveFileIfDoesNotExist(
            string source,
            string destination,
            bool pretend)
        {
            var moved = CopyOrMove(source, destination, pretend, File.Move);
            Debug.WriteLineIf(!pretend && moved, $"Moved {source} -> {destination}");
            return moved;
        }

        private bool CopyOrMove(
            string source,
            string destination,
            bool pretend,
            Action<string, string, bool> copyOrMove)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(source));
            }

            if (string.IsNullOrWhiteSpace(destination))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(destination));
            }

            if (File.Exists(destination))
            {
                Debug.WriteLine($"Skipped {source} -> {destination}");
                return false;
            }
            if (!pretend)
            {
                copyOrMove(source, destination, false);
            }
            return true;
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

        bool IFileSystem.IsDirectory(string path)
        {
            return File.GetAttributes(path).HasFlag(FileAttributes.Directory);
        }
    }
}