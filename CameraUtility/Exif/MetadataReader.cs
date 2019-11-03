﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MetadataExtractor;
using NotNullAttribute = JetBrains.Annotations.NotNullAttribute;

namespace CameraUtility.Exif
{
    public sealed class MetadataReader : IMetadataReader
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
            [NotNull] 
            [SuppressMessage("ReSharper", "PrivateFieldCanBeConvertedToLocalVariable")]
            private readonly string _name;

            public MetadataExtractorTagAdapter([NotNull] Tag metadataExtractorTag)
            {
                if (metadataExtractorTag is null)
                {
                    throw new ArgumentNullException(nameof(metadataExtractorTag));
                }

                Directory = metadataExtractorTag.DirectoryName;
                Value = metadataExtractorTag.Description ?? string.Empty;
                Type = metadataExtractorTag.Type;
                _name = metadataExtractorTag.Name ?? string.Empty;
            }

            public string Directory { get; }
            public string Value { get; }
            public int Type { get; }
        }
    }
}