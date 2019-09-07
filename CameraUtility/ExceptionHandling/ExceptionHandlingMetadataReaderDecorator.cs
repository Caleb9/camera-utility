using System;
using System.Collections.Generic;
using CameraUtility.Exif;
using JetBrains.Annotations;
using MetadataExtractor;

namespace CameraUtility.ExceptionHandling
{
    internal sealed class ExceptionHandlingMetadataReaderDecorator
        : IMetadataReader
    {
        [NotNull] private readonly IMetadataReader _decorated;

        internal ExceptionHandlingMetadataReaderDecorator(
            [NotNull] IMetadataReader decorated)
        {
            _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
        }

        IEnumerable<ITag> IMetadataReader.ExtractTags(string filePath)
        {
            try
            {
                return _decorated.ExtractTags(filePath);
            }
            catch (ImageProcessingException exception)
            {
                throw new InvalidFileException(filePath, exception);
            }
        }
    }
}