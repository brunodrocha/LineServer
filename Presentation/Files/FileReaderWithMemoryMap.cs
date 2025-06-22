namespace Presentation.Files
{
    using System.IO;
    using System.IO.MemoryMappedFiles;

    using Microsoft.Extensions.Options;

    public class FileReaderWithMemoryMap : IFileReader, IDisposable
    {
        private readonly string path;
        private readonly ILineIndexer lineIndexer;
        private readonly MemoryMappedFile memMappedFile;
        private readonly long totalFileLength;

        public FileReaderWithMemoryMap(IOptions<FileReaderSettings> settings, ILineIndexer lineIndexer)
        {
            this.path = settings?.Value?.FilePath ?? throw new ArgumentNullException(nameof(settings));
            this.lineIndexer = lineIndexer;
            this.memMappedFile = MemoryMappedFile.CreateFromFile(path, FileMode.Open, null, 0, MemoryMappedFileAccess.Read);
            this.totalFileLength = new FileInfo(this.path).Length;
        }

        public async Task<string?> GetLineAsync(int index)
        {
            var lineStart = this.lineIndexer.GetLineStart(index);

            long length;

            if (index < this.lineIndexer.TotalLines - 1)
            {
                length = this.lineIndexer.GetLineStart(index + 1) - lineStart;
            }
            else
            {
                length = totalFileLength - lineStart;
            }

            using var view = this.memMappedFile.CreateViewStream(lineStart, length, MemoryMappedFileAccess.Read);
            using var stream = new StreamReader(view);

            return await stream.ReadLineAsync();
        }

        public void Dispose()
        {
            this.memMappedFile.Dispose();
        }
    }
}
