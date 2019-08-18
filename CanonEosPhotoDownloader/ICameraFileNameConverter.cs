using JetBrains.Annotations;

namespace CanonEosPhotoDownloader
{
    public interface ICameraFileNameConverter
    {
        (string destinationDirectory, string destinationFileFullName) Convert(
            [NotNull] string cameraFilePath,
            [NotNull] string destinationRootPath);
    }
}