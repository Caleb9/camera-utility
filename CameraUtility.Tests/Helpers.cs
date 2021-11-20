using CameraUtility.Exif;
using Moq;

namespace CameraUtility.Tests;

internal static class Helpers
{
    internal static IEnumerable<ITag> NewImageFileTags(
        string createdDateOriginal,
        string subSeconds)
    {
        return new[]
        {
            NewCreatedDateTimeOriginalTag(createdDateOriginal),
            NewSubSecondsTag(subSeconds)
        };

        static ITag NewCreatedDateTimeOriginalTag(string value)
        {
            return Mock.Of<ITag>(t =>
                t.Directory == "Exif SubIFD" && t.Type == 0x9003 && t.Value == value);
        }

        static ITag NewSubSecondsTag(string value)
        {
            return Mock.Of<ITag>(t =>
                t.Directory == "Exif SubIFD" && t.Type == 0x9291 && t.Value == value);
        }
    }

    internal static IEnumerable<ITag> NewQuickTimeFileTags(string value)
    {
        return new[]
        {
            Mock.Of<ITag>(t =>
                t.Directory == "QuickTime Movie Header" && t.Type == 0x3 && t.Value == value)
        };
    }
}