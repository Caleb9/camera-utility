namespace CameraUtility
{
    public sealed record CameraFilePath(string Value) :
        TypedOption<string>(Value)
    {
        internal string GetExtension()
        {
            var extensionIndex = Value.LastIndexOf('.');
            return extensionIndex < 0 ? string.Empty : Value[extensionIndex..];
        }
    }
}