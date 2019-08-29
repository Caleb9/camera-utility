using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CameraUtility.Exif;
using JetBrains.Annotations;

namespace CameraUtility.CameraFiles
{
    /// <summary>
    ///     Mp4 video (Android and Canon)
    /// </summary>
    public sealed class VideoFile : AbstractCameraFile, ICameraFile
    {
        private const int QuickTimeCreatedTag = 0x3;

        public VideoFile([NotNull] string fullName, IEnumerable<ITag> exifTags)
            : base(fullName)
        {
            var createdTag = 
                exifTags.First(t => t.Directory == "QuickTime Movie Header" && t.Type == QuickTimeCreatedTag);
            Created = DateTime.ParseExact(createdTag.Value, "ddd MMM dd HH.mm.ss yyyy", CultureInfo.InvariantCulture);
        }

        public override DateTime Created { get; }
        public override string DestinationNamePrefix => "VID_";
    }
}