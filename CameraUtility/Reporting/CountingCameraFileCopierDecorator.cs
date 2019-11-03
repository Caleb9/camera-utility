using System;

namespace CameraUtility.Reporting
{
    internal sealed class CountingCameraFileCopierDecorator
        : ICameraFileCopier
    {
        private readonly ICameraFileCopier _decorated;
        private readonly Report _report;

        public CountingCameraFileCopierDecorator(
            ICameraFileCopier decorated,
            Report report)
        {
            _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
            _report = report ?? throw new ArgumentNullException(nameof(report));
        }

        void ICameraFileCopier.ExecuteCopyFile(
            string cameraFilePath,
            string destinationDirectoryRoot)
        {
            try
            {
                _decorated.ExecuteCopyFile(cameraFilePath, destinationDirectoryRoot);
                _report.IncrementNumberOfFilesWithValidMetadata();
            }
            catch (Exception exception)
            {
                _report.AddExceptionForFile(cameraFilePath, exception);
                throw;
            }
        }
    }
}