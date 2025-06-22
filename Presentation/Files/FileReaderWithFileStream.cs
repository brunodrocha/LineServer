namespace Presentation.Files
{
    using System.IO;
    using System.Text;
    using System.Threading;

    using Microsoft.Extensions.Options;

    public class FileReaderWithFileStream : IFileReader, IDisposable
    {
        private readonly string path;
        private readonly ILineIndexer lineIndexer;
        private readonly FileStream stream;
        private readonly SemaphoreSlim semaphore;

        public FileReaderWithFileStream(IOptions<FileReaderSettings> settings, ILineIndexer lineIndexer)
        {
            this.path = settings?.Value?.FilePath ?? throw new ArgumentNullException(nameof(settings));
            this.semaphore = new SemaphoreSlim(1, 1);
            this.lineIndexer = lineIndexer;
            this.stream = new FileStream(this.path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
        }

        public async Task<string?> GetLineAsync(int index)
        {
            var lineStart = this.lineIndexer.GetLineStart(index);

            await this.semaphore.WaitAsync();

            try
            {
                this.stream.Seek(lineStart, SeekOrigin.Begin);
                using var reader = new StreamReader(this.stream, Encoding.ASCII, false, 1024, true);

                return await reader.ReadLineAsync();
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        public void Dispose()
        {
            this.stream.Dispose();
        }
    }
}
