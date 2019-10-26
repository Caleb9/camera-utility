using System;
using JetBrains.Annotations;

namespace CameraUtility.Reporting
{
    internal sealed class CountingCameraFileCopierDecorator
        : ICameraFileCopier
    {
        [NotNull] private readonly ICameraFileCopier _decorated;
        [NotNull] private readonly Report _report;

        public CountingCameraFileCopierDecorator(
            [NotNull] ICameraFileCopier decorated,
            [NotNull] Report report)
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