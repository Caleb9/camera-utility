namespace CameraUtility.Commands.ImageFilesTransfer.Options
{
    internal sealed record SourcePath(string Value) :
        TypeWrapper<string>(Value);
}