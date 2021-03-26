using System;

namespace CameraUtility.CameraFiles
{
    internal interface ICameraFile
    {
        CameraFilePath FullName { get; }
        string Extension { get; }
        DateTime Created { get; }

        string DestinationNamePrefix { get; }
    }
}