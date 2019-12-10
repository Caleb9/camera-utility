using System;
using System.Collections.Generic;
using CameraUtility.CameraFiles;
using CameraUtility.Exif;

namespace CameraUtility.ExceptionHandling
{
    internal sealed class ExceptionHandlingCameraFileFactoryDecorator
        : ICameraFileFactory
    {
        private readonly ICameraFileFactory _decorated;

        internal ExceptionHandlingCameraFileFactoryDecorator(
            ICameraFileFactory decorated)
        {
            _decorated = decorated;
        }

        ICameraFile ICameraFileFactory.Create(
            string filePath,
            IEnumerable<ITag> metadata)
        {
            try
            {
                return _decorated.Create(filePath, metadata);
            }
            catch (InvalidOperationException exception)
            {
                throw new InvalidMetadataException(
                    $"Failed to find necessary metadata in {filePath}",
                    exception);
            }
            catch (FormatException exception)
            {
                throw new InvalidMetadataException(
                    $"Failed to parse metadata for {filePath}",
                    exception);
            }
        }
    }
}