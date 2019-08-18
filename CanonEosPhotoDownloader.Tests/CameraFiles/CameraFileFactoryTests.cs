using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CanonEosPhotoDownloader.CameraFiles;
using CanonEosPhotoDownloader.Exif;
using Moq;
using NUnit.Framework;

namespace CanonEosPhotoDownloader.Tests.CameraFiles
{
    [TestFixture]
    [TestOf(typeof(CameraFileFactory))]
    [ExcludeFromCodeCoverage]
    public sealed class CameraFileFactoryTests
    {
        [Test]
        [TestOf(nameof(ICameraFileFactory.Create))]
        public void Create_ExtensionIsJpg_ReturnsImageFile()
        {
            /* Arrange */
            ICameraFileFactory sut = new CameraFileFactory();
            var tags = new[]
            {
                Mock.Of<ITag>(t =>
                    t.Directory == "Exif SubIFD" && t.Type == 0x9003 && t.Value == "2010:11:12 13:14:15"),
                Mock.Of<ITag>(t => t.Directory == "Exif SubIFD" && t.Type == 0x9291 && t.Value == "42")
            };

            /* Act */
            var result = sut.Create("file.jpg", tags);

            /* Assert */
            Assert.IsInstanceOf<ImageFile>(result);
        }

        [Test]
        [TestOf(nameof(ICameraFileFactory.Create))]
        public void Create_ExtensionIsCr2_ReturnsImageFile()
        {
            /* Arrange */
            ICameraFileFactory sut = new CameraFileFactory();
            var tags = new[]
            {
                Mock.Of<ITag>(t => 
                    t.Directory == "Exif SubIFD" && t.Type == 0x9003 && t.Value == "2010:11:12 13:14:15"),
                Mock.Of<ITag>(t => t.Directory == "Exif SubIFD" && t.Type == 0x9291 && t.Value == "42")
            };

            /* Act */
            var result = sut.Create("file.cr2", tags);

            /* Assert */
            Assert.IsInstanceOf<ImageFile>(result);
        }

        [Test]
        [TestOf(nameof(ICameraFileFactory.Create))]
        public void Create_ExtensionIsDng_ReturnsDngImageFile()
        {
            /* Arrange */
            ICameraFileFactory sut = new CameraFileFactory();
            var tags = new[]
            {
                Mock.Of<ITag>(t => t.Type == 0x9003 && t.Value == "2010:11:12 13:14:15"),
                Mock.Of<ITag>(t => t.Type == 0x9291 && t.Value == "42")
            };

            /* Act */
            var result = sut.Create("file.dng", tags);

            /* Assert */
            Assert.IsInstanceOf<DngImageFile>(result);
        }

        [Test]
        [TestOf(nameof(ICameraFileFactory.Create))]
        public void Create_ExtensionIsMp4_ReturnsDngVideoFile()
        {
            /* Arrange */
            ICameraFileFactory sut = new CameraFileFactory();
            var tags = new[]
            {
                Mock.Of<ITag>(t => 
                    t.Directory == "QuickTime Movie Header" && t.Type == 0x3 && t.Value == "Fri Jun 13 14.15.16 1980")
            };

            /* Act */
            var result = sut.Create("file.mp4", tags);

            /* Assert */
            Assert.IsInstanceOf<VideoFile>(result);
        }
    }
}