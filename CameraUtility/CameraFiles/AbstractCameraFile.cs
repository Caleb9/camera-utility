using System;
using System.Diagnostics;
using CameraUtility.Utils;
using JetBrains.Annotations;

namespace CameraUtility.CameraFiles
{
    [DebuggerDisplay("{FullName} {Created}")]
    public abstract class AbstractCameraFile : ICameraFile
    {
        protected AbstractCameraFile([NotNull] string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(fullName));
            }

            FullName = fullName;
            Extension = FileNameUtil.GetExtension(fullName);
            if (string.IsNullOrWhiteSpace(Extension))
            {
                throw new ArgumentException($"File {fullName} has no extension", nameof(fullName));
            }
        }

        public string FullName { get; }
        public string Extension { get; }
        public abstract DateTime Created { get; }
        public abstract string DestinationNamePrefix { get; }

        public override string ToString()
        {
            return FullName;
        }
    }
}