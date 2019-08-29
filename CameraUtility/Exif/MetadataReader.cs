using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using MetadataExtractor;

namespace CameraUtility.Exif
{
    public sealed class MetadataReader : IMetadataReader
    {
        IEnumerable<ITag> IMetadataReader.ExtractTags(string filePath)
        {
            return ImageMetadataReader.ReadMetadata(filePath)
                .SelectMany(d => d.Tags)
                .Select(t => new MetadataExtractorTagAdapter(t));
        }

        [DebuggerDisplay("[{Directory}] {_name} : {Value}")]
        private sealed class MetadataExtractorTagAdapter : ITag
        {
            /// <summary>
            ///     Currently used only by debugger.
            /// </summary>
            [NotNull] 
            [SuppressMessage("ReSharper", "PrivateFieldCanBeConvertedToLocalVariable")]
            private readonly string _name;

            public MetadataExtractorTagAdapter([NotNull] Tag metadataExtractorTag)
            {
                if (metadataExtractorTag == null)
                {
                    throw new ArgumentNullException(nameof(metadataExtractorTag));
                }

                Directory = metadataExtractorTag.DirectoryName;
                Value = metadataExtractorTag.Description;
                Type = metadataExtractorTag.Type;
                _name = metadataExtractorTag.Name;
            }

            public string Directory { get; }
            public string Value { get; }
            public int Type { get; }
        }
    }
}