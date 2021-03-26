namespace CameraUtility.Commands.ImageFilesTransfer.Options
{
    internal sealed record DestinationDirectory(string Value) :
        TypedOption<string>(Value);
}