using System;
using System.Diagnostics.CodeAnalysis;
using CanonEosPhotoDownloader.CameraFiles;
using CanonEosPhotoDownloader.Exif;
using Moq;
using NUnit.Framework;

namespace CanonEosPhotoDownloader.Tests.CameraFiles
{
    [TestFixture]
    [TestOf(typeof(ImageFile))]
    [ExcludeFromCodeCoverage]
    public sealed class ImageFileTests
    {
        [Test]
        public void Ctor_ParsesCorrectly()
        {
            /* Arrange */
            var dateTimeOriginalTagStub =
                Mock.Of<ITag>(
                    t => t.Type == 0x9003 && t.Value == "2010:11:12 13:14:15" && t.Directory == "Exif SubIFD");
            var subSecondTag =
                Mock.Of<ITag>(
                    t => t.Type == 0x9291 && t.Value == "16" && t.Directory == "Exif SubIFD");

            /* Act */
            var result = new ImageFile("fakeFileName.jpg", new []{ dateTimeOriginalTagStub, subSecondTag });

            /* Assert */
            Assert.AreEqual(new DateTime(2010, 11, 12, 13, 14, 15, 160), result.Created);
        }

        [Test]
        [TestOf(nameof(ICameraFile.DestinationNamePrefix))]
        public void DestinationNamePrefix_IsImg()
        {
            /* Arrange */
            var dateTimeOriginalTagStub =
                Mock.Of<ITag>(
                    t => t.Type == 0x9003 && t.Value == "2010:11:12 13:14:15" && t.Directory == "Exif SubIFD");
            var subSecondTag =
                Mock.Of<ITag>(
                    t => t.Type == 0x9291 && t.Value == "16" && t.Directory == "Exif SubIFD");
            var sut = new ImageFile("fakeFileName.jpg", new []{ dateTimeOriginalTagStub, subSecondTag });

            /* Act */
            var result = sut.DestinationNamePrefix;

            /* Assert */
            Assert.AreEqual("IMG_", result);
        }
    }
}