using System;
using System.Diagnostics;

namespace CameraUtility.CameraFiles
{
    [DebuggerDisplay("{FullName} {Created}")]
    public abstract class AbstractCameraFile
    {
        protected AbstractCameraFile(
            CameraFilePath fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(fullName));
            }

            FullName = fullName;
            Extension = fullName.GetExtension();
            if (string.IsNullOrWhiteSpace(Extension))
            {
                throw new ArgumentException($"File {fullName} has no extension", nameof(fullName));
            }
        }

        public CameraFilePath FullName { get; }
        public string Extension { get; }
        public abstract DateTime Created { get; }
        public abstract string DestinationNamePrefix { get; }

        public override string ToString()
        {
            return FullName;
        }
    }
}