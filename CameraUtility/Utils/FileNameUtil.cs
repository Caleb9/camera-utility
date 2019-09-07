using System;
using JetBrains.Annotations;

namespace CameraUtility.Utils
{
    public static class FileNameUtil
    {
        [NotNull]
        public static string GetExtension([NotNull] string imagePath)
        {
            if (imagePath == null)
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