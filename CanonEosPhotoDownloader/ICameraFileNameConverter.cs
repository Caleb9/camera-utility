using JetBrains.Annotations;

namespace CanonEosPhotoDownloader
{
    /// <summary>
    ///     Generates new name for a file, based on its type (image or video) and creation date (extracted from Exif
    ///     metadata).
    /// </summary>
    public interface ICameraFileNameConverter
    {
        /// <summary>
        ///     Returns a tuple containing destination directory (including date-sub-directory) and new file name (full
        ///     path).
        /// </summary>
        (string destinationDirectory, string destinationFileFullName) Convert(
            [NotNull] string cameraFilePath,
            [NotNull] string destinationRootPath);
    }
}