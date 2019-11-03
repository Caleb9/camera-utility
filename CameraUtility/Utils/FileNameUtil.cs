using System;

namespace CameraUtility.Utils
{
    public static class FileNameUtil
    {
        public static string GetExtension(string imagePath)
        {
            if (imagePath is null)
            {
                throw new ArgumentNullException(nameof(imagePath));
            }

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