using System;
using System.IO;
using System.Linq;

public class DataLogger
{
    // Array of tuples. value 0: key, value 1: content
    private (string, Func<string>)[] logMethods;
    private string path;

    public DataLogger((string, Func<string>)[] _logMethods, string _path)
    {
        logMethods = _logMethods;
        path = _path;

        // Create directory to file if it does not exist
        Directory.CreateDirectory(Path.GetDirectoryName(path));

        if (!File.Exists(path))
        {
            File.Create(path);
        }

        File.WriteAllText(path, string.Empty);
        WriteHeader();
    }

    private void WriteHeader()
    {
        string content = string.Join(",", logMethods.Select(x => x.Item1).ToArray());
        WriteLine(content);
    }

    private void WriteLine(string content)
    {
        File.AppendAllText(path, content + Environment.NewLine);
    }

    public void Log()
    {
        string content = string.Join(",", logMethods.Select(x => x.Item2()).ToArray());
        WriteLine(content);
    }
}
