using System.Collections.Generic;
using CameraUtility.Exif;
using JetBrains.Annotations;

namespace CameraUtility.CameraFiles
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