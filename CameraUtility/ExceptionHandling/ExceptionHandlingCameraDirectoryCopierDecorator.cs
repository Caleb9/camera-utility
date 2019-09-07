using System;
using System.Threading;
using JetBrains.Annotations;

namespace CameraUtility.ExceptionHandling
{
    internal sealed class ExceptionHandlingCameraDirectoryCopierDecorator
        : ICameraDirectoryCopier
    {
        [NotNull] private readonly ICameraDirectoryCopier _decorated;

        internal ExceptionHandlingCameraDirectoryCopierDecorator(
            [NotNull] ICameraDirectoryCopier decorated)
        {
            _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
        }

        void ICameraDirectoryCopier.CopyCameraFiles(
            string sourceDirectory,
            string destinationDirectoryRoot,
            bool pretend,
            CancellationToken cancellationToken)
        {
            try
            {
                _decorated.CopyCameraFiles(sourceDirectory, destinationDirectoryRoot, pretend, cancellationToken);
            }
            catch (Exception exception) when (!(exception is OperationCanceledException))
            {
                Console.WriteLine("An error occurred. Further processing aborted.");
                Console.Error.WriteLine(exception.Message);
            }
        }
    }
}