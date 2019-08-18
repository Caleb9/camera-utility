using System;
using System.Diagnostics.CodeAnalysis;
using CanonEosPhotoDownloader.CameraFiles;
using CanonEosPhotoDownloader.Exif;
using Moq;
using NUnit.Framework;

namespace CanonEosPhotoDownloader.Tests.CameraFiles
{
    [TestFixture]
    [TestOf(typeof(VideoFile))]
    [ExcludeFromCodeCoverage]
    public class VideoFileTests
    {
        [Test]
        public void Ctor_ParsesCorrectly()
        {
            /* Arrange */
            var createdTag =
                Mock.Of<ITag>(t =>
                    t.Directory == "QuickTime Movie Header" && t.Type == 0x3 && t.Value == "Fri Jun 13 14.15.16 1980");

            /* Act */
            var result = new VideoFile("fakeFileName.mp4", new []{ createdTag });

            /* Assert */
            Assert.AreEqual(new DateTime(1980, 06, 13, 14, 15, 16), result.Created);
        }
    }
}