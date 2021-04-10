using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace CameraUtility.Commands.ImageFilesTransfer.Options
{
    [DebuggerDisplay("{Value}")]
    [SuppressMessage("ReSharper", "UseNameofExpression")]
    internal sealed record Overwrite(bool Value) :
        TypedOption<bool>(Value);
}