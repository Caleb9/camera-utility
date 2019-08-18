using System.Collections.Generic;
using CanonEosPhotoDownloader.Exif;
using JetBrains.Annotations;

namespace CanonEosPhotoDownloader.CameraFiles
{
    public interface ICameraFileFactory
    {
        [NotNull]
        ICameraFile Create(
            [NotNull] string filePath,
            [NotNull] [ItemNotNull] IEnumerable<ITag> metadata);
    }
}