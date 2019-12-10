using System;

namespace CameraUtility.ExceptionHandling
{
    internal sealed class ExceptionHandlingCameraFileCopierDecorator
        : ICameraFileCopier
    {
        private readonly bool _continueOnError;
        private readonly ICameraFileCopier _decorated;

        public ExceptionHandlingCameraFileCopierDecorator(
            ICameraFileCopier decorated,
            bool continueOnError)
        {
            _decorated = decorated;
            _continueOnError = continueOnError;
        }

        public void ExecuteCopyFile(
            string cameraFilePath,
            string destinationDirectoryRoot)
        {
            try
            {
                _decorated.ExecuteCopyFile(cameraFilePath, destinationDirectoryRoot);
            }
            catch (Exception exception) when (_continueOnError &&
                                              (exception is InvalidMetadataException ||
                                               exception is InvalidFileException))
            {
            }
        }
    }
}