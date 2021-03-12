namespace CameraUtility.Commands.ImageFilesTransfer.Options
{
    internal sealed record DestinationDirectory(string Value) :
        TypeWrapper<string>(Value);
}