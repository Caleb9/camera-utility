namespace CameraUtility.Exif
{
    /// <summary>
    ///     Isolates MetadataExtractor.Tag type.
    /// </summary>
    public interface ITag
    {
        int Type { get; }

        /// <summary>
        ///     Exif metadata directory.
        /// </summary>
        string Directory { get; }

        string Value { get; }
    }
}