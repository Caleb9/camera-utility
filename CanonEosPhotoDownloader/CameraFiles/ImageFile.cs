using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CanonEosPhotoDownloader.Exif;
using JetBrains.Annotations;

namespace CanonEosPhotoDownloader.CameraFiles
{
    /// <summary>
    ///     Jpeg (Android and Canon) or Cr2 raw Canon photo.
    /// </summary>
    public sealed class ImageFile : AbstractCameraFile, ICameraFile
    {
        /// <summary>
        ///     <see href="https://www.media.mit.edu/pia/Research/deepview/exif.html" />
        /// </summary>
        private const int DateTimeOriginalTagType = 0x9003;

        private const int SubSecondTagType = 0x9291;

        public ImageFile([NotNull] string fullName, [NotNull] IEnumerable<ITag> exifTags)
            : base(fullName)
        {
            if (exifTags == null)
            {
                throw new ArgumentNullException(nameof(exifTags));
            }

            var enumeratedExifTags = exifTags.ToList();
            var dateTimeOriginal = 
                enumeratedExifTags.First(t => t.Directory == "Exif SubIFD" && t.Type == DateTimeOriginalTagType);
            Created = DateTime.ParseExact(
                dateTimeOriginal.Value,
                "yyyy:MM:dd HH:mm:ss",
                CultureInfo.InvariantCulture);
            var subSeconds = enumeratedExifTags.First(t => t.Directory == "Exif SubIFD" && t.Type == SubSecondTagType);
            Created = Created.AddMilliseconds(ToMilliseconds(int.Parse(subSeconds.Value)));
        }

        public override DateTime Created { get; }
        public override string DestinationNamePrefix => "IMG_";

        private int ToMilliseconds(int subSeconds)
        {
            return subSeconds * 10;
        }
    }
}