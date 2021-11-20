using System.Globalization;
using CameraUtility.Exif;
using CSharpFunctionalExtensions;

namespace CameraUtility.CameraFiles;

/// <summary>
///     Raw Android photo.
/// </summary>
internal sealed class DngImageFile :
    AbstractCameraFile, ICameraFile
{
    /// <summary>
    ///     <see href="https://www.media.mit.edu/pia/Research/deepview/exif.html" />
    /// </summary>
    private const int DateTimeOriginalTagType = 0x9003;

    private DngImageFile(
        CameraFilePath fullName,
        DateTime created)
        : base(fullName)
    {
        Created = created;
    }

    public override DateTime Created { get; }
    public override string DestinationNamePrefix => "IMG_";

    internal static Result<ICameraFile> Create(
        CameraFilePath fullName,
        IEnumerable<ITag> exifTags)
    {
        var dateTimeOriginal = exifTags.FirstOrDefault(t => t.Type == DateTimeOriginalTagType);
        if (dateTimeOriginal is null)
        {
            return Result.Failure<ICameraFile>("Metadata not found");
        }

        if (DateTime.TryParseExact(
                dateTimeOriginal.Value,
                "yyyy:MM:dd HH:mm:ss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsedDateTime))
        {
            return new DngImageFile(fullName, parsedDateTime);
        }

        return Result.Failure<ICameraFile>("Invalid metadata");
    }
}