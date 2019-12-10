﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CameraUtility.Reporting
{
    /// <summary>
    ///     Collects information about number of files processed.
    /// </summary>
    internal sealed class Report
    {
        private readonly bool _filesAreMoved;

        /// <summary>
        ///     Accumulates list of errors for printing the final summary.
        /// </summary>
        private readonly List<string> _errors = new List<string>();

        /// <summary>
        ///     Files found per directory. Each execution of <see cref="ICameraFilesFinder.FindCameraFiles" /> will add
        ///     to the overall sum of processed files.
        /// </summary>
        /// <remarks>
        ///     This is not thread safe. If this class is used concurrently, ConcurrentDictionary should be used.
        /// </remarks>
        private readonly Dictionary<string, int> _filesFound = new Dictionary<string, int>();

        /// <summary>
        ///     List of files that already exist in the destination.
        /// </summary>
        private readonly HashSet<(string source, string destination)> _filesSkipped =
            new HashSet<(string, string)>();

        /// <summary>
        ///     Changes the summary message to indicate if program is copying or moving files.
        /// </summary>
        /// <param name="filesAreMoved">True if files are to be moved; False if files are to be copied</param>
        public Report(
            bool filesAreMoved)
        {
            _filesAreMoved = filesAreMoved;
        }
        
        private int FilesFound => _filesFound.Values.Sum();

        private int FilesMetadataRead { get; set; }
        private int FilesCopied { get; set; }

        internal void AddNumberOfFilesFoundIn(
            string directory,
            int numberOfFiles)
        {
            if (directory is null)
            {
                throw new ArgumentNullException(nameof(directory));
            }

            _filesFound.Add(directory, numberOfFiles);
        }

        internal void IncrementNumberOfFilesWithValidMetadata()
        {
            FilesMetadataRead++;
        }

        internal void IncrementNumberOfFilesCopied()
        {
            FilesCopied++;
        }

        internal void AddExceptionForFile(
            string fileName,
            Exception exception)
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (exception is null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            _errors.Add($"{fileName}: {exception.Message}");
        }

        internal void AddSkippedFile(
            string sourceFileName,
            string destinationFileName)
        {
            if (string.IsNullOrWhiteSpace(sourceFileName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(sourceFileName));
            }

            if (string.IsNullOrWhiteSpace(destinationFileName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(destinationFileName));
            }

            _filesSkipped.Add((sourceFileName, destinationFileName));
        }

        internal void PrintReport(bool printErrors = true)
        {
            var currentColor = Console.ForegroundColor;
            try
            {
                PrintSkippedFiles();
                if (printErrors)
                {
                    PrintErrors();
                }

                PrintSummary();
            }
            finally
            {
                Console.ForegroundColor = currentColor;
            }
        }

        private void PrintSkippedFiles()
        {
            if (!_filesSkipped.Any())
            {
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Skipped {_filesSkipped.Count} because they already exist at the destination");
            foreach (var (source, destination) in _filesSkipped)
            {
                Console.WriteLine($"{source} exists as {destination}");
            }
        }

        private void PrintErrors()
        {
            if (!_errors.Any())
            {
                return;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Following errors occurred:");
            foreach (var error in _errors)
            {
                Console.WriteLine(error);
            }
        }

        private void PrintSummary()
        {
            Console.ForegroundColor = _errors.Any() ? ConsoleColor.Red : ConsoleColor.Green;
            var copiedOrMoved = _filesAreMoved ? "Moved" : "Copied";
            Console.WriteLine(
                $"Found {FilesFound} camera files. " +
                $"Processed {FilesMetadataRead}. " +
                $"Skipped {_filesSkipped.Count}. " +
                $"{copiedOrMoved} {FilesCopied}.");
        }
    }
}