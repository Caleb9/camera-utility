using System.Collections.Generic;
using CameraUtility.Exif;
using MetadataExtractor;

namespace CameraUtility.ExceptionHandling
{
    internal sealed class ExceptionHandlingMetadataReaderDecorator
        : IMetadataReader
    {
        private readonly IMetadataReader _decorated;

        internal ExceptionHandlingMetadataReaderDecorator(
            IMetadataReader decorated)
        {
            _decorated = decorated;
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