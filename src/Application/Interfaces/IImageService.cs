using Domain.DTOs;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IImageService
    {
        Task<List<string>> ProcessUploadedImages(List<IFormFile> files);
        Task<Stream?> GetResizedImage(string imageId, string size);
        Task<ImageMetadataDto?> GetMetadata(string imageId);
    }
}
