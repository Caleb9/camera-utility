using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CameraUtility.Exif;
using JetBrains.Annotations;

namespace CameraUtility.CameraFiles
{
    /// <summary>
    ///     Raw Android photo.
    /// </summary>
    public sealed class DngImageFile : AbstractCameraFile, ICameraFile
    {
        /// <summary>
        ///     <see href="https://www.media.mit.edu/pia/Research/deepview/exif.html" />
        /// </summary>
        private const int DateTimeOriginalTagType = 0x9003;

        internal DngImageFile(
            [NotNull] string fullName,
            [NotNull] IEnumerable<ITag> exifTags)
            : base(fullName)
        {
            if (exifTags == null)
            {
                throw new ArgumentNullException(nameof(exifTags));
            }

            var dateTimeOriginal = exifTags.First(t => t.Type == DateTimeOriginalTagType);
            Created = DateTime.ParseExact(
                dateTimeOriginal.Value,
                "yyyy:MM:dd HH:mm:ss",
                CultureInfo.InvariantCulture);
        }

        public override DateTime Created { get; }
        public override string DestinationNamePrefix => "IMG_";
    }
}