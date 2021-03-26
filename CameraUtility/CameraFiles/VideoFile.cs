using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CameraUtility.Exif;
using CSharpFunctionalExtensions;

namespace CameraUtility.CameraFiles
{
    /// <summary>
    ///     Mp4 video (Android and Canon)
    /// </summary>
    internal sealed class VideoFile :
        AbstractCameraFile, ICameraFile
    {
        private const int QuickTimeCreatedTag = 0x3;

        private VideoFile(
            CameraFilePath fullName,
            DateTime created)
            : base(fullName)
        {
            Created = created;
        }

        internal static Result<ICameraFile> Create(
            CameraFilePath fullName,
            IEnumerable<ITag> exifTags)
        {
            var createdTag =
                exifTags.FirstOrDefault(t => t.Directory == "QuickTime Movie Header" && t.Type == QuickTimeCreatedTag);
            if (createdTag is null)
            {
                return Result.Failure<ICameraFile>("Metadata not found");
            }
            if (DateTime.TryParseExact(
                createdTag.Value,
                "ddd MMM dd HH.mm.ss yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsedDateTime))
            {
                return new VideoFile(fullName, parsedDateTime);
            }
            return Result.Failure<ICameraFile>("Invalid metadata");
        }
        public override DateTime Created { get; }
        public override string DestinationNamePrefix => "VID_";
    }
}