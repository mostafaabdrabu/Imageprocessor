using Application.Interfaces;

namespace Infrastructure.Services
{
    public class FileSystemStorageService : IStorageService
    {
        private readonly string _basePath = Path.Combine(Directory.GetCurrentDirectory(), "Storage");
        public FileSystemStorageService() 
        {
            Directory.CreateDirectory(Path.Combine(_basePath, "Original"));
            Directory.CreateDirectory(Path.Combine(_basePath, "Resized"));
            Directory.CreateDirectory(Path.Combine(_basePath, "Metadata"));
        }
        public async Task SaveFileAsync(string path, Stream content)
        {
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory!);

            using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            content.Position = 0;
            await content.CopyToAsync(fileStream);
        }

        public bool FileExists(string path) => File.Exists(path);

        public async Task<Stream?> GetFileAsync(string path)
        {
            if (!File.Exists(path)) return null;
            return new FileStream(path, FileMode.Open, FileAccess.Read);
        }

        public async Task<string> ReadTextFileAsync(string path)
        {
            return await File.ReadAllTextAsync(path);
        }

        public async Task WriteTextFileAsync(string path, string content)
        {
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory!);

            await File.WriteAllTextAsync(path, content);
        }

        public string GeneratePath(string folder, string fileName)
        {
            return Path.Combine(_basePath, folder, fileName);
        }
    }
}
