using System;

namespace CameraUtility.ExceptionHandling
{
    internal sealed class InvalidFileException
        : Exception
    {
        internal InvalidFileException(
            string filePath,
            Exception innerException)
            : base($"Failed to extract metadata from file {filePath}", innerException)
        {
        }
    }
}