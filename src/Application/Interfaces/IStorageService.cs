
namespace Application.Interfaces
{
    public interface IStorageService
    {
        Task SaveFileAsync(string path, Stream content);
        bool FileExists(string path);
        Task<Stream?> GetFileAsync(string path);
        Task<string> ReadTextFileAsync(string path);
        Task WriteTextFileAsync(string path, string content);
        string GeneratePath(string folder, string fileName);
    }
}
