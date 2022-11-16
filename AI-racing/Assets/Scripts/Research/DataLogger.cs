using System;
using System.IO;
using System.Linq;

public class DataLogger
{
    // Array of tuples. value 0: key, value 1: content
    private (string, Func<string>)[] logMethods;
    private string path;

    public DataLogger((string, Func<string>)[] _logMethods, string _path, string initialText = "")
    {
        logMethods = _logMethods;
        path = _path;

        if (!File.Exists(path))
        {
            // Create directory to file if it does not exist
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            // Create file and then close it, which allows text to be written to 
            // the newly created file
            File.Create(path).Close();
        }

        File.WriteAllText(path, string.Empty);

        if (initialText.Length > 0)
        {
            WriteLine(initialText);
            WriteRowSeparator();
        }

        WriteHeader();
    }

    public void WriteHeader()
    {
        string content = string.Join(",", logMethods.Select(x => x.Item1).ToArray());
        WriteLine(content);
    }

    private void WriteLine(string content)
    {
        File.AppendAllText(path, content + Environment.NewLine);
    }

    public void WriteRowSeparator()
    {
        WriteLine(GetSeparatorRow());
    }

    public void Log()
    {
        string content = string.Join(",", logMethods.Select(x => x.Item2()).ToArray());
        WriteLine(content);
    }

    public static string GetSeparatorRow()
    {
        return new String('-', 40);
    }
}
