using System.Collections.Generic;
using CanonEosPhotoDownloader.Exif;
using JetBrains.Annotations;

namespace CanonEosPhotoDownloader.CameraFiles
{
    /// <summary>
    ///     Selects <see cref="ICameraFile"/> implementation based on file's extension.
    /// </summary>
    public interface ICameraFileFactory
    {
        /// <summary>
        ///     Creates new <see cref="ICameraFile"/> implementation based on file's extension.
        /// </summary>
        [NotNull]
        ICameraFile Create(
            [NotNull] string filePath,
            [NotNull] [ItemNotNull] IEnumerable<ITag> metadata);
    }
}