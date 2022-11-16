using System;
using System.IO;

public static class IO 
{
    public static void CreateFile(string path)
    {
        if (!File.Exists(path))
        {
            // Create directory to file if it does not exist
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            // Create file and then close it, which allows text to be written to 
            // the newly created file
            File.Create(path).Close();
        }
    }

    public static void WriteLine(string path, string content)
    {
        File.AppendAllText(path, content + Environment.NewLine);
    }
}
