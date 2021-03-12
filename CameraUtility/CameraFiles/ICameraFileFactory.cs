using System.Collections.Generic;
using CameraUtility.Exif;

namespace CameraUtility.CameraFiles
{
    /// <summary>
    ///     Selects <see cref="ICameraFile" /> implementation based on file's extension.
    /// </summary>
    public interface ICameraFileFactory
    {
        /// <summary>
        ///     Creates new <see cref="ICameraFile" /> implementation based on file's extension.
        /// </summary>
        ICameraFile Create(
            string filePath,
            IEnumerable<ITag> metadata);
    }
}