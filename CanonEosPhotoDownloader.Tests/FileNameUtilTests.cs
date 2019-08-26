using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace CanonEosPhotoDownloader.Tests
{
    [TestFixture]
    [TestOf(typeof(FileNameUtil))]
    [ExcludeFromCodeCoverage]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public class FileNameUtilTests
    {
        [TestCase("file.ext", ".ext")]
        [TestCase("file.asd.ext", ".ext")]
        [TestCase("file", "")]
        [TestCase("", "")]
        [TestCase("file.EXT", ".EXT")]
        [TestOf(nameof(FileNameUtil.GetExtension))]
        public void GetExtension_InputNotNull_ReturnsCorrectExtension(
            string input, string expected)
        {
            /* Act */
            var result = FileNameUtil.GetExtension(input);

            /* Assert */
            Assert.AreEqual(expected, result);
        }

        [Test]
        [TestOf(nameof(FileNameUtil.GetExtension))]
        public void GetExtension_InputNull_Throws()
        {
            /* Act */
            void TestDelegate() => FileNameUtil.GetExtension(null);

            /* Assert */
            Assert.Throws<ArgumentNullException>(TestDelegate);
        }
    }
}