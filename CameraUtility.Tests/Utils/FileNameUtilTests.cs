using System;
using System.Diagnostics.CodeAnalysis;
using CameraUtility.Utils;
using NUnit.Framework;

namespace CameraUtility.Tests.Utils
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
    }
}