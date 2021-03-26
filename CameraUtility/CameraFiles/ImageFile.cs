using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CameraUtility.Exif;
using CSharpFunctionalExtensions;

namespace CameraUtility.CameraFiles
{
    /// <summary>
    ///     Jpeg (Android and Canon) or Cr2 raw Canon photo.
    /// </summary>
    internal sealed class ImageFile
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

        private ImageFile(
            CameraFilePath fullName,
            DateTime created)
            : base(fullName)
        {
            Created = created;
        }

        public override DateTime Created { get; }
        public override string DestinationNamePrefix => "IMG_";

        internal static Result<ICameraFile> Create(
            CameraFilePath fullName,
            IEnumerable<ITag> exifTags)
        {
            var enumeratedExifTags = exifTags.ToList();
            var dateTimeOriginalResult = FindCreatedDateTimeTag(enumeratedExifTags);
            if (dateTimeOriginalResult.IsFailure)
            {
                return Result.Failure<ICameraFile>(dateTimeOriginalResult.Error);
            }
            var parsedDateTimeResult = ParseCreatedDateTime(dateTimeOriginalResult.Value);
            if (parsedDateTimeResult.IsFailure)
            {
                return Result.Failure<ICameraFile>(parsedDateTimeResult.Error);
            }

            return new ImageFile(
                fullName,
                parsedDateTimeResult.Value.AddMilliseconds(FindSubSeconds(enumeratedExifTags)));
        }

        private static Result<ITag> FindCreatedDateTimeTag(
            IList<ITag> exifTags)
        {
            var tag = exifTags.FirstOrDefault(t => t.Type == DateTimeOriginalTagType)
                      /* Try fallback tag, if not found then an exception will be thrown */
                      ?? exifTags.FirstOrDefault(t => t.Type == FallbackDateTimeTagType);
            return
                tag is not null
                    ? Result.Success(tag)
                    : Result.Failure<ITag>("Metadata not found");
        }

        private static Result<DateTime> ParseCreatedDateTime(
            ITag dateTimeOriginal)
        {
            if (DateTime.TryParseExact(
                dateTimeOriginal.Value,
                "yyyy:MM:dd HH:mm:ss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsedDateTime))
            {
                return parsedDateTime;
            }
            return Result.Failure<DateTime>("Invalid metadata");
        }

        private static int FindSubSeconds(
            IEnumerable<ITag> exifTags)
        {
            var subSeconds = exifTags.FirstOrDefault(t => t.Type == SubSecondTagType);
            return subSeconds is null ? 0 : ToMilliseconds(int.Parse(subSeconds.Value));
        }

        private static int ToMilliseconds(
            int subSeconds)
        {
            return subSeconds * 10;
        }
    }
}