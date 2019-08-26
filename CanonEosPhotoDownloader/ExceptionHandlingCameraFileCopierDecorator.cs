using System;
using JetBrains.Annotations;

namespace CanonEosPhotoDownloader
{
    internal sealed class ExceptionHandlingCameraFileCopierDecorator
        : ICameraFileCopier
    {
        [NotNull] private readonly ICameraFileCopier _decorated;

        internal ExceptionHandlingCameraFileCopierDecorator(
            [NotNull] ICameraFileCopier decorated)
        {
            _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
        }

        void ICameraFileCopier.CopyCameraFiles(
            string sourceDirectory,
            string destinationDirectoryRoot,
            bool dryRun)
        {
            try
            {
                _decorated.CopyCameraFiles(sourceDirectory, destinationDirectoryRoot, dryRun);
            }
            catch (Exception exception)
            {
                Console.WriteLine("An error occurred. Further processing aborted.");
                Console.Error.WriteLine(exception.Message);
            }
        }
    }
}