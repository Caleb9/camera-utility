using System;
using CanonEosPhotoDownloader.CameraFiles;
using CanonEosPhotoDownloader.Exif;
using CanonEosPhotoDownloader.FileSystemIsolation;
using JetBrains.Annotations;

namespace CanonEosPhotoDownloader
{
    public sealed class CameraFileNameConverter
        : ICameraFileNameConverter
    {
        [NotNull] private readonly ICameraFileFactory _cameraFileFactory;
        [NotNull] private readonly IFileSystem _fileSystem;
        [NotNull] private readonly IMetadataReader _metadataReader;

        public CameraFileNameConverter(
            [NotNull] IMetadataReader metadataReader,
            [NotNull] ICameraFileFactory cameraFileFactory,
            [NotNull] IFileSystem fileSystem)
        {
            _metadataReader = metadataReader ?? throw new ArgumentNullException(nameof(metadataReader));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _cameraFileFactory = cameraFileFactory;
        }

        (string destinationDirectory, string destinationFileFullName) ICameraFileNameConverter.Convert(
            string cameraFilePath,
            string destinationRootPath)
        {
            var cameraFile = GetCameraFile(cameraFilePath);
            var destinationDirectory = GetDestinationDirectory(destinationRootPath, cameraFile);
            var destinationFileFullName = GetDestinationFileFullName(destinationDirectory, cameraFile);

            return (destinationDirectory, destinationFileFullName);
        }

        [NotNull]
        private ICameraFile GetCameraFile(
            [NotNull] string cameraFilePath)
        {
            var metadataTags = _metadataReader.ExtractTags(cameraFilePath);
            return _cameraFileFactory.Create(cameraFilePath, metadataTags);
        }

        [NotNull]
        private string GetDestinationDirectory(
            [NotNull] string destinationRootPath,
            [NotNull] ICameraFile cameraFile)
        {
            var destinationSubDirectory = GetDateSubDirectoryName(cameraFile.Created);
            return _fileSystem.CombinePaths(destinationRootPath, destinationSubDirectory);
        }

        [NotNull]
        private string GetDateSubDirectoryName(
            DateTime created)
        {
            return $"{created.Year:0000}_{created.Month:00}_{created.Day:00}";
        }

        [NotNull]
        private string GetDestinationFileFullName(
            [NotNull] string destinationDirectory, 
            [NotNull] ICameraFile cameraFile)
        {
            var fileName = NewCameraFileName(cameraFile);
            var destinationFileFullName = _fileSystem.CombinePaths(destinationDirectory, fileName);
            return destinationFileFullName;
        }

        private string NewCameraFileName(
            ICameraFile cameraFile)
        {
            return $"{cameraFile.DestinationNamePrefix}{GetDateForFileName(cameraFile.Created)}" +
                   $"_{GetTimeForFileName(cameraFile.Created)}{cameraFile.Extension}";
        }

        [NotNull]
        private string GetDateForFileName(
            DateTime created)
        {
            return $"{created.Year:0000}{created.Month:00}{created.Day:00}";
        }

        [NotNull]
        private string GetTimeForFileName(
            DateTime created)
        {
            return $"{created.Hour:00}{created.Minute:00}{created.Second:00}{created.Millisecond:000}";
        }
    }
}