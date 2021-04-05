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
    public sealed class ImageFile
        : AbstractCameraFile, ICameraFile
    {
        /// <summary>
        ///     <see href="https://www.media.mit.edu/pia/Research/deepview/exif.html" />
        /// </summary>
        private const int DateTimeOriginalTagType = 0x9003;

        /// <summary>
        ///     Some older cameras don't use the 0x9003. We will try to read it from 0x0132 tag (ModifyDate).
        /// </summary>
        private const int FallbackDateTimeTagType = 0x0132;

        private const int SubSecTimeOriginalTagType = 0x9291;

        private ImageFile(
            CameraFilePath fullName,
            DateTime created)
            : base(fullName)
        {
            Created = created;
        }

        public override DateTime Created { get; }
        public override string DestinationNamePrefix => "IMG_";

        public static Result<ICameraFile> Create(
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
                parsedDateTimeResult.Value.Add(FindSubSeconds(enumeratedExifTags)));
        }

        private static Result<ITag> FindCreatedDateTimeTag(
            IList<ITag> exifTags)
        {
            var tag = exifTags.FirstOrDefault(t => t.Type == DateTimeOriginalTagType)
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

        private static TimeSpan FindSubSeconds(
            IEnumerable<ITag> exifTags)
        {
            var subSeconds = exifTags.FirstOrDefault(t => t.Type == SubSecTimeOriginalTagType);
            return subSeconds is null ? TimeSpan.Zero : ToMilliseconds(subSeconds.Value);
        }

        /// <summary>
        ///     EXIF specifies that SubSecOriginal tag contains "fractions" of a second. Depending on length of the
        ///     value a different fractional unit can be used, e.g. "042" is 42 milliseconds (0.042 of a second) but
        ///     "42" is 420 milliseconds (0.42 of a second).
        /// </summary>
        private static TimeSpan ToMilliseconds(
            string subSeconds)
        {
            if (int.TryParse(subSeconds, out var tagIntValue) is false)
            {
                return TimeSpan.Zero;
            }
            var subSecondDenominator = Math.Pow(10, subSeconds.Trim().Length);
            var millisecondMultiplier = 1000 / subSecondDenominator;
            return TimeSpan.FromMilliseconds(tagIntValue * millisecondMultiplier);
        }
    }
}