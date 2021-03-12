namespace CameraUtility.Commands.ImageFilesTransfer.Options
{
    internal sealed record SkipDateSubdirectory(bool Value) :
        TypeWrapper<bool>(Value);
}