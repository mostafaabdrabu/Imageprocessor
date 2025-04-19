using Domain.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return BadRequest("No images uploaded.");

            List<string> result = await _imageService.ProcessUploadedImages(files);
            return Ok(result);
        }

        [HttpGet("{imageId}/download/{size}")]
        public async Task<IActionResult> DownloadResizedImage(string imageId, string size)
        {
            Stream? imageStream = await _imageService.GetResizedImage(imageId, size);
            if (imageStream == null) return NotFound();

            return File(imageStream, "image/webp");
        }

        [HttpGet("{imageId}/metadata")]
        public async Task<IActionResult> GetImageMetadata(string imageId)
        {
            ImageMetadataDto? metadata = await _imageService.GetMetadata(imageId);
            if (metadata == null) return NotFound();

            return Ok(metadata);
        }
    }
}
