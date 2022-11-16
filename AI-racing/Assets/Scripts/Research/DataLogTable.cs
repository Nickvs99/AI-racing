using System;
using System.IO;
using System.Linq;

using UnityEngine;

/// <summary>
/// Maps data log files through an ID. This ID can be found by the parameters used.
/// </summary>
public class DataLogTable
{
    private string path;
    private int nextID;

    public DataLogTable(string _path)
    {
        path = _path;

        InitFile();

        nextID = GetLastID() + 1;
    }

    private void InitFile()
    {
        IO.CreateFile(path);

        if(File.ReadLines(path).Count() == 0)
        {
            WriteHeader();
        }
    }

    private void WriteHeader()
    {
        string header = "ID,populationSize,selectionName,weightInitName,biasInitName,activationName,mutationRate,weightMutateName,biasMutateName,hiddenLayers,fov,nrays";
        WriteLine(header);
    }

    private void WriteLine(string content)
    {
        IO.WriteLine(path, content);
    }

    private int GetLastID()
    {
        if(File.ReadLines(path).Count() == 1)
        {
            return 0;
        }

        string lastLine = File.ReadLines(path).Last();
        string[] lineSeperated = lastLine.Split(',');

        return Int32.Parse(lineSeperated[0]);
    }

    public void AddNewID(EvolutionParameters parameters)
    {
        string row = $"{nextID},{parameters}";
        WriteLine(row);

        nextID++;
    }

    public int GetNewID()
    {
        return nextID;
    }

    public bool CheckFileExists(EvolutionParameters parameters)
    {
        foreach (string line in File.ReadLines(path))
        {
            if(line.Contains(parameters.ToString()))
            {
                return true;
            }
        }

        return false;
    }
}
