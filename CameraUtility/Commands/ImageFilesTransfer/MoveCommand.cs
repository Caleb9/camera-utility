namespace CameraUtility.Commands.ImageFilesTransfer;

internal sealed class MoveCommand :
    AbstractTransferImageFilesCommand
{
    private const string DescriptionText =
        "Moves suppported image and video files to destination directory " +
        "and renames them by date recorded in EXIF metadata.";

    public MoveCommand(
        OptionsHandler handler)
        : base("move", DescriptionText, handler)
    {
        AddAlias("mv");
    }
}