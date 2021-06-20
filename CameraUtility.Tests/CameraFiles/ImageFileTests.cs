using System;
using CameraUtility.CameraFiles;
using CameraUtility.Exif;
using FluentAssertions;
using Xunit;

namespace CameraUtility.Tests.CameraFiles
{
    public sealed class ImageFileTests
    {
        [Theory]
        [InlineData("42", 420)]
        [InlineData("042", 42)]
        [InlineData("042000", 42)]
        public void SubSecondsOriginal_tag_gets_converted_to_milliseconds(
            string subSecondsTagValue,
            int expectedMilliseconds)
        {
            var exifTags = new[]
            {
                new Tag
                {
                    Type = 0x9003,
                    Value = "2021:04:05 10:56:13"
                },
                new Tag
                {
                    Type = 0x9291,
                    Value = subSecondsTagValue
                }
            };
            
            var sut = ImageFile.Create(new CameraFilePath("file.jpg"), exifTags).Value;

            var expected = new DateTime(2021, 04, 05, 10, 56, 13).AddMilliseconds(expectedMilliseconds);
            sut.Created.Should().Be(expected);
        }

        private sealed class Tag :
            ITag
        {
            public int Type { get; internal init; }
            public string Directory => string.Empty;
            public string Value { get; internal init; } = string.Empty;
        }

        private sealed record TestTag(string Value) :
            ITag
        {
            public int Type => 0x9003;
            public string Directory => string.Empty;
        }
    }
}