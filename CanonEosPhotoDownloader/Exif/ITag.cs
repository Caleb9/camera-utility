namespace CanonEosPhotoDownloader.Exif
{
    /// <summary>
    ///     Isolates MetadataExtractor.Tag type.
    /// </summary>
    public interface ITag
    {
        int Type { get; }
        string Directory { get; }
        string Value { get; }
    }
}