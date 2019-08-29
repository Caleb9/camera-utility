using System.Collections.Generic;
using JetBrains.Annotations;

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
        IEnumerable<ITag> ExtractTags([NotNull] string filePath);
    }
}