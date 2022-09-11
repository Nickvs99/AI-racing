using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveManager
{
    public static void SaveAgent(AgentData data)
    {
        Debug.Log("Data saved");
        string json = JsonUtility.ToJson(data);

        // Long term solution
        //string path = Application.persistentDataPath + "/agent.bin";

        // Short term solution
        string path = Path.Combine(Application.dataPath, "agent.json");

        StreamWriter writer = new StreamWriter(path);
        writer.Write(json);
        writer.Close();

    }

    public static AgentData LoadAgent()
    {
        string path = Path.Combine(Application.dataPath, "agent.json");

        StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        reader.Close();

        AgentData data = JsonUtility.FromJson<AgentData>(json);

        return data;
    }
}