using System;
using System.Threading;

namespace CameraUtility.ExceptionHandling
{
    internal sealed class ExceptionHandlingCameraDirectoryCopierDecorator
        : ICameraDirectoryCopier
    {
        private readonly ICameraDirectoryCopier _decorated;

        internal ExceptionHandlingCameraDirectoryCopierDecorator(
            ICameraDirectoryCopier decorated)
        {
            _decorated = decorated;
        }

        void ICameraDirectoryCopier.CopyCameraFiles(
            string sourcePath,
            string destinationDirectoryRoot,
            CancellationToken cancellationToken)
        {
            try
            {
                _decorated.CopyCameraFiles(sourcePath, destinationDirectoryRoot, cancellationToken);
            }
            catch (Exception exception) when (!(exception is OperationCanceledException))
            {
                Console.WriteLine("An error occurred. Further processing aborted.");
                Console.Error.WriteLine(exception.Message);
            }
        }
    }
}