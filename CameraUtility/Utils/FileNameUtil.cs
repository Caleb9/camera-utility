namespace CameraUtility.Utils
{
    internal static class FileNameUtil
    {
        internal static string GetExtension(string imagePath)
        {
            var extensionIndex = imagePath.LastIndexOf('.');
            return extensionIndex < 0 ? string.Empty : imagePath[extensionIndex..];
        }
    }
}