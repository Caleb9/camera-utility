namespace CameraUtility.CameraFiles;

public interface ICameraFile
{
    CameraFilePath FullName { get; }
    string Extension { get; }
    DateTime Created { get; }

    string DestinationNamePrefix { get; }
}