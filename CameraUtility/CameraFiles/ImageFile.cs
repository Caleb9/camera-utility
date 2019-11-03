using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CameraUtility.Exif;

namespace CameraUtility.CameraFiles
{
    /// <summary>
    ///     Jpeg (Android and Canon) or Cr2 raw Canon photo.
    /// </summary>
    public sealed class ImageFile
        : AbstractCameraFile, ICameraFile
    {
        /// <summary>
        ///     <see href="https://www.media.mit.edu/pia/Research/deepview/exif.html" />
        /// </summary>
        private const int DateTimeOriginalTagType = 0x9003;

        /// <summary>
        ///     Some older cameras don't use the 0x9003. We will try to read it from 0x0132 tag.
        /// </summary>
        private const int FallbackDateTimeTagType = 0x0132;

        private const int SubSecondTagType = 0x9291;

        public ImageFile(string fullName, IEnumerable<ITag> exifTags)
            : base(fullName)
        {
            if (exifTags is null)
            {
                throw new ArgumentNullException(nameof(exifTags));
            }

            var enumeratedExifTags = exifTags.ToList();
            var dateTimeOriginal = FindCreatedDateTimeTag(enumeratedExifTags);
            Created =
                ParseCreatedDateTime(dateTimeOriginal)
                    .AddMilliseconds(FindSubSeconds(enumeratedExifTags));
        }

        public override DateTime Created { get; }
        public override string DestinationNamePrefix => "IMG_";

        private ITag FindCreatedDateTimeTag(
            IList<ITag> exifTags)
        {
            return exifTags.FirstOrDefault(t => t.Type == DateTimeOriginalTagType)
                   /* Try fallback tag, if not found then an exception will be thrown */
                   ?? exifTags.First(t => t.Type == FallbackDateTimeTagType);
        }

        private DateTime ParseCreatedDateTime(
            ITag dateTimeOriginal)
        {
            return DateTime.ParseExact(
                dateTimeOriginal.Value,
                "yyyy:MM:dd HH:mm:ss",
                CultureInfo.InvariantCulture);
        }

        private int FindSubSeconds(
            IEnumerable<ITag> exifTags)
        {
            var subSeconds = exifTags.FirstOrDefault(t => t.Type == SubSecondTagType);
            return subSeconds is null ? 0 : ToMilliseconds(int.Parse(subSeconds.Value));
        }

        private int ToMilliseconds(
            int subSeconds)
        {
            return subSeconds * 10;
        }
    }
}