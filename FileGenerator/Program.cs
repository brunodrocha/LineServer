using System.Text;

public class Program
{
    private static void Main(string[] args)
    {
        string[] loremWords = ("Lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor incididunt ut labore et dolore magna aliqua").Split();
        var random = new Random();

        string filePath = GetArg(args, "--file-path");
        if (!long.TryParse(GetArg(args, "--file-size-gb"), out var fileSizeGB))
        {
            throw new ArgumentNullException("--file-size-gb");
        }

        long targetBytes = fileSizeGB * 1024 * 1024 * 1024;

        using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        using var writer = new StreamWriter(stream, Encoding.ASCII)
        {
            NewLine = "\n"
        };

        int lineNumber = 0;

        while (stream.Length < targetBytes)
        {
            int wordCount = random.Next(5, 30);
            var lineWords = new string[wordCount];

            for (int i = 0; i < wordCount; i++)
                lineWords[i] = loremWords[random.Next(loremWords.Length)];

            string line = $"{lineNumber++}: {string.Join(' ', lineWords)}";
            writer.WriteLine(line);
        }

        Console.WriteLine($"Generated file '{filePath}' with approx. {fileSizeGB}GB and {lineNumber} lines.");
    }

    private static string GetArg(string[] args, string argName)
    {
        var index = Array.IndexOf(args, argName);

        if (index < 0 || index + 1 >= args.Length)
        {
            throw new ArgumentNullException(argName);
        }

        return args[index + 1];
    }
}