using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CameraUtility.Commands.ImageFilesTransfer.Options;

namespace CameraUtility.Commands.ImageFilesTransfer.Output
{
    internal sealed class Report
    {
        private readonly HashSet<(string source, string destination)> _cameraFilesSkipped = new();
        private readonly List<string> _errors = new();
        private readonly TextWriter _textWriter;
        private long _cameraFilesFound;
        private long _cameraFilesProcessed;
        private int _cameraFilesTransferred;

        internal Report(
            TextWriter textWriter)
        {
            _textWriter = textWriter;
        }

        internal void HandleCameraFilesFound(
            object? sender,
            long count)
        {
            _cameraFilesFound = count;
        }

        internal void IncrementProcessed()
        {
            _cameraFilesProcessed++;
        }

        internal void IncrementTransferred(
            DryRun dryRun)
        {
            if (!dryRun)
            {
                _cameraFilesTransferred++;
            }
        }

        internal void AddSkippedFile(
            string sourceFileName,
            string destinationFileName)
        {
            _cameraFilesSkipped.Add((sourceFileName, destinationFileName));
        }

        internal void AddExceptionForFile(
            string fileName,
            Exception exception)
        {
            _errors.Add($"{fileName}: {exception.Message}");
        }

        internal void PrintReport(
            bool withErrors)
        {
            var currentColor = Console.ForegroundColor;
            try
            {
                PrintSkippedFiles();
                if (withErrors)
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
            if (!_cameraFilesSkipped.Any())
            {
                return;
            }

            _textWriter.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(
                $"Skipped {_cameraFilesSkipped.Count} file(s) because they already exist at the destination.");
            foreach (var (source, destination) in _cameraFilesSkipped)
            {
                stringBuilder.AppendLine($"{source} exists as {destination}");
            }

            _textWriter.Write(stringBuilder);
        }

        private void PrintErrors()
        {
            if (!_errors.Any())
            {
                return;
            }

            _textWriter.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Following errors occurred:");
            foreach (var error in _errors)
            {
                stringBuilder.AppendLine(error);
            }

            _textWriter.Write(stringBuilder);
        }

        private void PrintSummary()
        {
            _textWriter.WriteLine();
            Console.ForegroundColor = _errors.Any() ? ConsoleColor.Red : ConsoleColor.Green;
            _textWriter.WriteLine(
                $"Found {_cameraFilesFound} camera files. " +
                $"Processed {_cameraFilesProcessed}. " +
                $"Skipped {_cameraFilesSkipped.Count}. " +
                $"Transferred {_cameraFilesTransferred}.");
        }
    }
}