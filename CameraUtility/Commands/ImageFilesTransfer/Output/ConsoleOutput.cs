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
            CameraFilePath? sourceCameraFilePath,
            string error)
        {
            var message =
                sourceCameraFilePath is not null
                    ? $"Failed {sourceCameraFilePath}: {error}"
                    : error;
            WriteLine(message, ConsoleColor.Red);
        }

        internal void HandleCreatedDirectory(
            string directory)
        {
            WriteLine(
                $"Created {directory}",
                ConsoleColor.Blue);
        }

        internal void HandleFileCopied(
            CameraFilePath sourcePath,
            CameraFilePath destinationPath)
        {
            WriteLine($"{sourcePath} -> {destinationPath}");
        }

        internal void HandleFileMoved(
            CameraFilePath sourcePath,
            CameraFilePath destinationPath)
        {
            WriteLine($"{sourcePath} -> {destinationPath}");
        }

        internal void HandleFileSkipped(
            CameraFilePath sourcePath,
            CameraFilePath destinationPath)
        {
            WriteLine(
                $"Skipped {sourcePath} ({destinationPath} already exists)",
                ConsoleColor.Yellow);
        }

        internal void HandleException(
            CameraFilePath sourceCameraFilePath,
            Exception exception)
        {
            WriteLine(
                $"Failed {sourceCameraFilePath}: {exception.Message}",
                ConsoleColor.Red);
        }
    }
}