namespace CameraUtility.Commands.Check.Options
{
    internal sealed record SourcePaths(string[] Values) :
        TypedOption<string[]>(Values);
}