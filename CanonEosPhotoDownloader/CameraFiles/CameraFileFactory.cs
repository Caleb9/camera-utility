using System;
using System.Collections.Generic;
using CanonEosPhotoDownloader.Exif;

namespace CanonEosPhotoDownloader.CameraFiles
{
    public sealed class CameraFileFactory : ICameraFileFactory
    {
        ICameraFile ICameraFileFactory.Create(string filePath, IEnumerable<ITag> metadata)
        {
            switch (FileNameUtil.GetExtension(filePath))
            {
                case ".jpg":
                case ".cr2":
                    return new ImageFile(filePath, metadata);
                case ".dng":
                    return new DngImageFile(filePath, metadata);
                case ".mp4":
                    return new VideoFile(filePath, metadata);
                default:
                    throw new InvalidPathException($"Unknown file type {filePath}");
            }
        }

        private sealed class InvalidPathException : Exception
        {
            internal InvalidPathException(string message, Exception innerException = null)
                : base(message, innerException)
            {
            }
        }
    }
}