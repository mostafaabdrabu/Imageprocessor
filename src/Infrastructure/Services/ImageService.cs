using Domain.DTOs;
using Domain.Constants;
using MetadataExtractor;
using SixLabors.ImageSharp;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using MetadataExtractor.Formats.Exif;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Webp;

namespace Infrastructure.Services
{
    public class ImageService : IImageService
    {
        private readonly IStorageService _storageService;

        public ImageService(IStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task<List<string>> ProcessUploadedImages(List<IFormFile> files)
        {
            List<string> ids = new();

            foreach (IFormFile file in files)
            {
                if (!IsValidFileSize(file))
                    continue;

                string extension = Path.GetExtension(file.FileName).ToLower();
                if (!IsValidFileExtension(extension))
                    continue;

                string imageId = Guid.NewGuid().ToString();
                string originalPath = _storageService.GeneratePath(FileSystemFolders.Original, $"{imageId}{extension}");

                using MemoryStream memoryStream = new();
                await file.CopyToAsync(memoryStream);
                await _storageService.SaveFileAsync(originalPath, memoryStream);

                await ResizeAndConvert(originalPath, imageId);
                await ExtractAndSaveMetadata(originalPath, imageId);

                ids.Add(imageId);
            }

            return ids;
        }

        private async Task ResizeAndConvert(string originalPath, string imageId)
        {
            using Image image = await Image.LoadAsync(originalPath);

            Dictionary<string, int> sizes = new()
            {
                { "phone", 480 },
                { "tablet", 768 },
                { "desktop", 1920 }
            };

            foreach (KeyValuePair<string, int> size in sizes)
            {
                Image clone = image.Clone(ctx => ctx.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(size.Value, size.Value)
                }));

                string outputPath = _storageService.GeneratePath(FileSystemFolders.Resized, $"{imageId}_{size.Key}.webp");

                using MemoryStream memoryStream = new();
                await clone.SaveAsync(memoryStream, new WebpEncoder());
                await _storageService.SaveFileAsync(outputPath, memoryStream);
            }
        }

        private async Task ExtractAndSaveMetadata(string imagePath, string imageId)
        {
            IReadOnlyList<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(imagePath);
            ExifSubIfdDirectory? exif = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
            GpsDirectory? geoDirectory = directories.OfType<GpsDirectory>().FirstOrDefault();

            ImageMetadataDto metadata = new()
            {
                CameraMake = exif?.GetDescription(ExifDirectoryBase.TagMake),
                CameraModel = exif?.GetDescription(ExifDirectoryBase.TagModel),
                Latitude = geoDirectory?.GetGeoLocation()?.Latitude,
                Longitude = geoDirectory?.GetGeoLocation()?.Longitude
            };

            string metadataJson = System.Text.Json.JsonSerializer.Serialize(metadata);
            string path = _storageService.GeneratePath(FileSystemFolders.Metadata, $"{imageId}.json");

            await _storageService.WriteTextFileAsync(path, metadataJson);
        }

        public async Task<Stream?> GetResizedImage(string imageId, string size)
        {
            string path = _storageService.GeneratePath(FileSystemFolders.Resized, $"{imageId}_{size}.webp");
            return await _storageService.GetFileAsync(path);
        }

        public async Task<ImageMetadataDto?> GetMetadata(string imageId)
        {
            string path = _storageService.GeneratePath(FileSystemFolders.Metadata, $"{imageId}.json");

            if (!_storageService.FileExists(path))
                return null;

            string json = await _storageService.ReadTextFileAsync(path);
            return System.Text.Json.JsonSerializer.Deserialize<ImageMetadataDto>(json);
        }


        private static bool IsValidFileExtension(string extension)
        {
            return (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".webp");
        }

        private static bool IsValidFileSize(IFormFile file)
        {
            return file.Length < 2 * 1024 * 1024;
        }
    }
}
