using System;
using System.Threading;

namespace CameraUtility
{
    /// <summary>
    ///     Orchestrator for finding images and videos in source directory, and copying them to destination directory
    ///     (with changed name).
    /// </summary>
    public sealed class CameraDirectoryCopier
        : ICameraDirectoryCopier
    {
        private readonly ICameraFilesFinder _cameraFilesFinder;
        private readonly ICameraFileCopier _cameraFileCopier;

        public CameraDirectoryCopier(
            ICameraFilesFinder cameraFilesFinder,
            ICameraFileCopier cameraFileCopier)
        {
            _cameraFilesFinder = cameraFilesFinder;
            _cameraFileCopier = cameraFileCopier;
        }

        void ICameraDirectoryCopier.CopyCameraFiles(
            string sourceDirectory,
            string destinationDirectoryRoot,
            CancellationToken cancellationToken)
        {
            foreach (var cameraFilePath in _cameraFilesFinder.FindCameraFiles(sourceDirectory))
            {
                cancellationToken.ThrowIfCancellationRequested();
                _cameraFileCopier.ExecuteCopyFile(cameraFilePath, destinationDirectoryRoot);
            }
        }
    }
}