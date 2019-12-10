using System;

namespace CameraUtility.Utils
{
    public static class FileNameUtil
    {
        public static string GetExtension(string imagePath)
        {
            try
            {
                return imagePath.Substring(imagePath.LastIndexOf('.'));
            }
            catch (ArgumentOutOfRangeException)
            {
                return string.Empty;
            }
        }
    }
}