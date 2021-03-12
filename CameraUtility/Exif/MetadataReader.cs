using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MetadataExtractor;

namespace CameraUtility.Exif
{
    internal sealed class MetadataReader : IMetadataReader
    {
        IEnumerable<ITag> IMetadataReader.ExtractTags(string filePath)
        {
            return ImageMetadataReader.ReadMetadata(filePath)
                .SelectMany(tagDirectory => tagDirectory.Tags)
                .Select(tag => new MetadataExtractorTagAdapter(tag));
        }

        [DebuggerDisplay("[{Directory}] {_name} : {Value}")]
        private sealed class MetadataExtractorTagAdapter : ITag
        {
            /// <summary>
            ///     Currently used only by debugger.
            /// </summary>
            [SuppressMessage("ReSharper", "PrivateFieldCanBeConvertedToLocalVariable")]
            private readonly string _name;

            public MetadataExtractorTagAdapter(Tag metadataExtractorTag)
            {
                Directory = metadataExtractorTag.DirectoryName;
                Value = metadataExtractorTag.Description ?? string.Empty;
                Type = metadataExtractorTag.Type;
                _name = metadataExtractorTag.Name;
            }

            public string Directory { get; }
            public string Value { get; }
            public int Type { get; }
        }
    }
}