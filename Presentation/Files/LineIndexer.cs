namespace Presentation.Files
{
    using System.Collections.Generic;

    using Microsoft.Extensions.Options;

    public class LineIndexer : ILineIndexer
    {
        private readonly string path;
        private List<long> indexes;

        public LineIndexer(IOptions<FileReaderSettings> settings)
        {
            this.path = settings?.Value?.FilePath ?? throw new ArgumentNullException(nameof(settings));
            this.indexes = new List<long>();
        }
        public int TotalLines => this.indexes.Count;

        public void BuildIndexes()
        {
            using var reader = new StreamReader(this.path);

            var lineIndexes = new List<long>
            {
                0
            };

            int c;
            long position = 0;
            while ((c = reader.Read()) != -1)
            {
                position++;
                if (c == '\n')
                {
                    lineIndexes.Add(position);
                }
            }

            this.indexes = lineIndexes;
        }

        public long GetLineStart(int index)
        {
            if (index < 0 || index >= this.indexes.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return this.indexes[index];
        }
    }
}
