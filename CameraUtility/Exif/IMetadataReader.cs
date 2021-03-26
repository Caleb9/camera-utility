using System.Collections.Generic;

namespace CameraUtility.Exif
{
    /// <summary>
    ///     Isolates 3-rd party MetadataExtractor library for accessing metadata of image and video files.
    /// </summary>
    public interface IMetadataReader
    {
        /// <summary>
        ///     Returns all tags found in file's Exif metadata.
        /// </summary>
        IEnumerable<ITag> ExtractTags(
            CameraFilePath filePath);
    }
}