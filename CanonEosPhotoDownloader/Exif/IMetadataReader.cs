using System.Collections.Generic;
using JetBrains.Annotations;

namespace CanonEosPhotoDownloader.Exif
{
    /// <summary>
    ///     Isolates 3-rd party MetadataExtractor library.
    /// </summary>
    public interface IMetadataReader
    {
        IEnumerable<ITag> ExtractTags([NotNull] string filePath);
    }
}