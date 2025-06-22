namespace Presentation.Files
{
    public interface IFileReader
    {
        Task<string?> GetLineAsync(int index);
    }
}
