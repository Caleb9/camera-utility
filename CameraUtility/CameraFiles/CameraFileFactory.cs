using System;
using System.Collections.Generic;
using CameraUtility.Exif;
using CSharpFunctionalExtensions;

namespace CameraUtility.CameraFiles
{
    internal sealed class CameraFileFactory
    {
        internal Result<ICameraFile> Create(
            CameraFilePath filePath,
            IEnumerable<ITag> metadata)
        {
            switch (filePath.GetExtension().ToLowerInvariant())
            {
                case ".jpg":
                case ".jpeg":
                case ".cr2":
                    return ImageFile.Create(filePath, metadata);
                case ".dng":
                    return DngImageFile.Create(filePath, metadata);
                case ".mp4":
                case ".mov":
                    return VideoFile.Create(filePath, metadata);
                default:
                    throw new InvalidPathException($"Unknown file type {filePath}");
            }
        }

        private sealed class InvalidPathException : Exception
        {
            internal InvalidPathException(string message, Exception? innerException = null)
                : base(message, innerException)
            {
            }
        }
    }
}