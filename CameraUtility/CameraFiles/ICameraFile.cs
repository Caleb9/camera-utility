using System;
using JetBrains.Annotations;

namespace CameraUtility.CameraFiles
{
    public interface ICameraFile
    {
        [NotNull] string FullName { get; }
        [NotNull] string Extension { get; }
        DateTime Created { get; }

        [NotNull] string DestinationNamePrefix { get; }
    }
}