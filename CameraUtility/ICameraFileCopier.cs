﻿using JetBrains.Annotations;

namespace CameraUtility
{
    public interface ICameraFileCopier
    {
        void ExecuteCopyFile(
            [NotNull] string cameraFilePath,
            [NotNull] string destinationDirectoryRoot);
    }
}