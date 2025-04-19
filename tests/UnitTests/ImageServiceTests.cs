using Moq;
using Application.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;

namespace UnitTests
{
    public class ImageServiceTests
    {
        private readonly Mock<IStorageService> _storageMock;
        private readonly ImageService _imageService;

        public ImageServiceTests()
        {
            _storageMock = new Mock<IStorageService>();
            _imageService = new ImageService(_storageMock.Object);
        }

        [Fact]
        public async Task ProcessUploadedImages_ShouldIgnoreInvalidFileTypes()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(1024);
            mockFile.Setup(f => f.FileName).Returns("test.txt");

            var files = new List<IFormFile> { mockFile.Object };

            // Act
            var result = await _imageService.ProcessUploadedImages(files);

            // Assert
            Assert.Empty(result);
            _storageMock.Verify(x => x.SaveFileAsync(It.IsAny<string>(), It.IsAny<Stream>()), Times.Never);
        }

        [Fact]
        public async Task GetResizedImage_ShouldReturnNull_WhenFileNotFound()
        {
            // Arrange
            _storageMock.Setup(s => s.GetFileAsync(It.IsAny<string>()))
                        .ReturnsAsync((Stream?)null);

            // Act
            var result = await _imageService.GetResizedImage("dummy-id", "phone");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetMetadata_ShouldReturnNull_WhenFileDoesNotExist()
        {
            // Arrange
            _storageMock.Setup(s => s.FileExists(It.IsAny<string>())).Returns(false);

            // Act
            var metadata = await _imageService.GetMetadata("nonexistent-id");

            // Assert
            Assert.Null(metadata);
        }
    }
}