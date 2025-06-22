namespace Presentation.Files
{
    public interface ILineIndexer
    {
        int TotalLines { get; }

        void BuildIndexes();

        long GetLineStart(int index);
    }
}
