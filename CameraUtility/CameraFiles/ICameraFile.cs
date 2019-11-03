using System;

namespace CameraUtility.CameraFiles
{
    public interface ICameraFile
    {
        string FullName { get; }
        string Extension { get; }
        DateTime Created { get; }

        string DestinationNamePrefix { get; }
    }
}