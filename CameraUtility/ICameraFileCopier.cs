namespace CameraUtility
{
    public interface ICameraFileCopier
    {
        void ExecuteCopyFile(
            string cameraFilePath,
            string destinationDirectoryRoot);
    }
}