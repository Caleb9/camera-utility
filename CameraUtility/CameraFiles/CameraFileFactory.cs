using System;
using System.Collections.Generic;
using CameraUtility.Exif;
using CameraUtility.Utils;

namespace CameraUtility.CameraFiles
{
    internal sealed class CameraFileFactory
    {
        internal ICameraFile Create(string filePath, IEnumerable<ITag> metadata)
        {
            switch (FileNameUtil.GetExtension(filePath).ToLowerInvariant())
            {
                case ".jpg":
                case ".jpeg":
                case ".cr2":
                    return new ImageFile(filePath, metadata);
                case ".dng":
                    return new DngImageFile(filePath, metadata);
                case ".mp4":
                case ".mov":
                    return new VideoFile(filePath, metadata);
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