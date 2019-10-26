using System;
using System.Threading;
using JetBrains.Annotations;

namespace CameraUtility
{
    /// <summary>
    ///     Orchestrator for finding images and videos in source directory, and copying them to destination directory
    ///     (with changed name).
    /// </summary>
    public sealed class CameraDirectoryCopier
        : ICameraDirectoryCopier
    {
        [NotNull] private readonly ICameraFilesFinder _cameraFilesFinder;
        [NotNull] private readonly ICameraFileCopier _cameraFileCopier;

        public CameraDirectoryCopier(
            [NotNull] ICameraFilesFinder cameraFilesFinder,
            [NotNull] ICameraFileCopier cameraFileCopier)
        {
            _cameraFilesFinder = cameraFilesFinder ?? throw new ArgumentNullException(nameof(cameraFilesFinder));
            _cameraFileCopier =
                cameraFileCopier ?? throw new ArgumentNullException(nameof(cameraFileCopier));
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