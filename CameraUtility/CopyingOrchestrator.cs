using System;
using System.Threading;

namespace CameraUtility
{
    /// <summary>
    ///     Orchestrator for finding images and videos in source directory, and copying them to destination directory
    ///     (with changed name).
    /// </summary>
    public sealed class CopyingOrchestrator
        : ICameraDirectoryCopier
    {
        private readonly ICameraFilesFinder _cameraFilesFinder;
        private readonly ICameraFileCopier _cameraFileCopier;

        public CopyingOrchestrator(
            ICameraFilesFinder cameraFilesFinder,
            ICameraFileCopier cameraFileCopier)
        {
            _cameraFilesFinder = cameraFilesFinder;
            _cameraFileCopier = cameraFileCopier;
        }

        void ICameraDirectoryCopier.CopyCameraFiles(
            string sourcePath,
            string destinationDirectoryRoot,
            CancellationToken cancellationToken)
        {
            foreach (var cameraFilePath in _cameraFilesFinder.FindCameraFiles(sourcePath))
            {
                cancellationToken.ThrowIfCancellationRequested();
                _cameraFileCopier.ExecuteCopyFile(cameraFilePath, destinationDirectoryRoot);
            }
        }
    }
}