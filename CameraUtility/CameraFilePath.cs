using System.Diagnostics;

namespace CameraUtility
{
    [DebuggerDisplay("{" + nameof(Value) + "}")]
    public sealed record CameraFilePath(string Value)
    {
        internal string GetExtension()
        {
            var extensionIndex = Value.LastIndexOf('.');
            return extensionIndex < 0 ? string.Empty : Value[extensionIndex..];
        }
        
        public static implicit operator string(
            CameraFilePath cameraFilePath)
        {
            return cameraFilePath.Value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}