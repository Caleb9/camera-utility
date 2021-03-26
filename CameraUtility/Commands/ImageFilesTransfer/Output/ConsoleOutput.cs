using System;
using System.IO;

namespace CameraUtility.Commands.ImageFilesTransfer.Output
{
    internal sealed class ConsoleOutput :
        ConsoleOutputBase
    {
        internal ConsoleOutput(
            TextWriter textWriter)
        : base(textWriter)
        {
        }

        internal void HandleError(
            string error)
        {
            WriteLine(
                error,
                ConsoleColor.Red);
        }

        internal void HandleCreatedDirectory(
            string directory)
        {
            WriteLine(
                $"Created {directory}",
                ConsoleColor.Blue);
        }

        internal void HandleFileCopied(
            string sourcePath,
            string destinationPath)
        {
            WriteLine($"{sourcePath} -> {destinationPath}");
        }

        internal void HandleFileMoved(
            string sourcePath,
            string destinationPath)
        {
            WriteLine($"{sourcePath} -> {destinationPath}");
        }

        internal void HandleFileSkipped(
            string sourcePath,
            string destinationPath)
        {
            WriteLine(
                $"Skipped {sourcePath} ({destinationPath} already exists)",
                ConsoleColor.Yellow);
        }

        internal void HandleException(
            string sourcePath,
            Exception exception)
        {
            WriteLine(
                $"Failed {sourcePath}: {exception.Message}",
                ConsoleColor.Red);
        }
    }
}