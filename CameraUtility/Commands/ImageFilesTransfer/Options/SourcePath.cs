namespace CameraUtility.Commands.ImageFilesTransfer.Options
{
    internal sealed record SourcePath(string Value) :
        TypedOption<string>(Value);
}