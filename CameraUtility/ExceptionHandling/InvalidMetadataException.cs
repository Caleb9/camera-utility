using System;

namespace CameraUtility.ExceptionHandling
{
    internal sealed class InvalidMetadataException
        : Exception
    {
        internal InvalidMetadataException(
            string message,
            Exception innerException)
            : base(message, innerException)
        {
        }
    }
}