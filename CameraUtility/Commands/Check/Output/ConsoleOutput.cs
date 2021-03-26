using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CameraUtility.Commands.Check.Output
{
    internal sealed class ConsoleOutput :
        ConsoleOutputBase
    {
        internal ConsoleOutput(
            TextWriter textWriter)
            : base(textWriter)
        {
        }

        internal void PrintError(
            string error)
        {
            WriteLine(error, ConsoleColor.Red);
        }

        internal void PrintSummary(
            long cameraFilesCounter,
            IReadOnlyCollection<CameraFilePath> cameraFilesWithoutMetadata)
        {
            Write($"Found {cameraFilesCounter} camera file(s).");
            if (cameraFilesCounter < 1)
            {
                WriteLine();
                return;
            }

            if (cameraFilesWithoutMetadata.Any() is false)
            {
                WriteLine(" All files contain metadata.", ConsoleColor.Green);
                return;
            }

            WriteLine(
                $" Missing metadata in {cameraFilesCounter - cameraFilesWithoutMetadata.Count} file(s).",
                ConsoleColor.Red);
            WriteLine("Following files are missing metadata:");
            foreach (var cameraFile in cameraFilesWithoutMetadata)
            {
                WriteLine(cameraFile);
            }
        }
    }
}