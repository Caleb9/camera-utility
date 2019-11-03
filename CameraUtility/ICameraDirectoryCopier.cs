using System.Threading;

namespace CameraUtility
{
    public interface ICameraDirectoryCopier
    {
        /// <summary>
        ///     Find images and videos in source directory, and copy them to destination directory (with changed name).
        ///     Files are copied into sub-directories of <paramref name="destinationDirectoryRoot"/>. Sub-directory
        ///     names are generated from files creation date (extracted from metadata), e.g.
        ///     {destinationDirectoryRoot}/2019_08_26/. File names are changed in the destination to a form
        ///     resembling Android file names, e.g. IMG_20190826_102233123.jpg or MVI_20190826_102233123.mp4. The time
        ///     portion of the name contains milliseconds to support photos taken with high-speed continuous shooting,
        ///     where more than one picture per second is taken.
        /// </summary>
        /// <param name="sourceDirectory">
        ///     Directory containing image and video files to be copied.
        /// </param>
        /// <param name="destinationDirectoryRoot">
        ///     Directory where files will be copied to auto-created sub-directories. Sub-directory names are generated
        ///     from files creation date (extracted from metadata), e.g. {destinationDirectoryRoot}/2019_08_26/.
        /// </param>
        /// <param name="cancellationToken"></param>
        void CopyCameraFiles(
            string sourceDirectory,
            string destinationDirectoryRoot,
            CancellationToken cancellationToken = default);
    }
}