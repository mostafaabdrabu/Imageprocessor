using Application.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace UnitTests
{
    public class ImageServiceTests
    {
        [Fact]
        public async Task ProcessUploadedImages_ShouldSkipInvalidFileTypes()
        {
            // Arrange
            var storageService = new Mock<IStorageService>();
            var service = new ImageService(storageService.Object);
            var mockFile = new Mock<IFormFile>();

            mockFile.Setup(f => f.Length).Returns(1024);
            mockFile.Setup(f => f.FileName).Returns("test.txt");

            var files = new List<IFormFile> { mockFile.Object };

            // Act
            var result = await service.ProcessUploadedImages(files);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task ProcessUploadedImages_ShouldReturnImageId_ForValidFile()
        {
            // Arrange
            var storageService = new Mock<IStorageService>();
            var service = new ImageService(storageService.Object);
            var mockFile = new Mock<IFormFile>();

            var content = new MemoryStream(new byte[] { 1, 2, 3 });
            mockFile.Setup(f => f.Length).Returns(content.Length);
            mockFile.Setup(f => f.FileName).Returns("test.jpg");
            mockFile.Setup(f => f.OpenReadStream()).Returns(content);
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
                .Returns<Stream, System.Threading.CancellationToken>((stream, token) => content.CopyToAsync(stream, token));

            var files = new List<IFormFile> { mockFile.Object };

            // Act
            var result = await service.ProcessUploadedImages(files);

            // Assert
            Assert.Single(result);
        }
    }
}