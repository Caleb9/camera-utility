namespace CameraUtility.Commands.ImageFilesTransfer;

internal sealed class CopyCommand :
    AbstractTransferImageFilesCommand
{
    private const string DescriptionText =
        "Copies suppported image and video files to destination directory " +
        "and renames them by date recorded in EXIF metadata.";

    public CopyCommand(
        OptionsHandler handler)
        : base("copy", DescriptionText, handler)
    {
        AddAlias("cp");
    }
}