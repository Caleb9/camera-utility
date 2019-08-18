using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AutoFixture;
using AutoFixture.AutoMoq;
using CanonEosPhotoDownloader.CameraFiles;
using CanonEosPhotoDownloader.Exif;
using CanonEosPhotoDownloader.FileSystemIsolation;
using Moq;
using NUnit.Framework;

namespace CanonEosPhotoDownloader.Tests
{
    [TestFixture]
    [TestOf(typeof(CameraFileNameConverter))]
    [ExcludeFromCodeCoverage]
    public class CameraFileNameConverterTests
    {
        private IFixture NewFixture()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture
                .Freeze<Mock<IFileSystem>>()
                .Setup(fs => fs.PathCombine(It.IsAny<string[]>()))
                .Returns<string[]>(paths => string.Join('/', paths));
            return fixture;
        }

        [Test]
        [TestOf(nameof(ICameraFileNameConverter.Convert))]
        public void Convert_ImageFile_DestinationSubDirectoryIsImageCreationDate()
        {
            var fixture = NewFixture();
            fixture
                .Freeze<Mock<ICameraFileFactory>>()
                .Setup(f => f.Create(It.IsAny<string>(), It.IsAny<IEnumerable<ITag>>()))
                .Returns(Mock.Of<ICameraFile>(f => f.Created == new DateTime(2019, 08, 25)));
            ICameraFileNameConverter sut = fixture.Create<CameraFileNameConverter>();

            var (result, _) = sut.Convert("sourceDir/IMG_1234.jpg", "destDir");

            Assert.AreEqual("destDir/2019_08_25", result);
        }

        [Test]
        [TestOf(nameof(ICameraFileNameConverter.Convert))]
        public void Convert_ImageFile_DestinationFileNameIsCorrect()
        {
            var fixture = NewFixture();
            fixture
                .Freeze<Mock<ICameraFileFactory>>()
                .Setup(f => f.Create(It.IsAny<string>(), It.IsAny<IEnumerable<ITag>>()))
                .Returns(
                    Mock.Of<ICameraFile>(f =>
                        f.DestinationNamePrefix == "IMG_" &&
                        f.Extension == ".jpg" &&
                        f.Created == new DateTime(2019, 08, 25, 14, 39, 42, 123)));
            ICameraFileNameConverter sut = fixture.Create<CameraFileNameConverter>();

            var (_, result) = sut.Convert("sourceDir/IMG_1234.jpg", "destDir");

            Assert.AreEqual("destDir/2019_08_25/IMG_20190825_143942123.jpg", result);
        }
        
        [TestCase(".jpg")]
        [TestCase(".cr2")]
        [TestCase(".mp4")]
        [TestCase(".dng")]
        [TestOf(nameof(ICameraFileNameConverter.Convert))]
        public void Convert_ImageFile_DestinationExtensionIsCorrect(
            string extension)
        {
            var fixture = NewFixture();
            fixture
                .Freeze<Mock<ICameraFileFactory>>()
                .Setup(f => f.Create(It.IsAny<string>(), It.IsAny<IEnumerable<ITag>>()))
                .Returns(
                    Mock.Of<ICameraFile>(f =>
                        f.Extension == extension));
            ICameraFileNameConverter sut = fixture.Create<CameraFileNameConverter>();

            var (_, result) = sut.Convert($"sourceDir/IMG_1234{extension}", "destDir");

            Assert.IsTrue(result.EndsWith(extension));
        }
    }
}