using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveManager
{
    public static void Save<T>(T data, string dataPath)
    {
        string saveFile = Application.persistentDataPath + "/" + dataPath;
        string json = JsonConvert.SerializeObject(data);
        File.WriteAllText(saveFile, json);        
    }

    public static T Load<T>(T data, string dataPath)
    {
        try
        {
            string saveFile = Application.persistentDataPath + "/" + dataPath;
            Debug.Log(saveFile);
            if (File.Exists(saveFile))
            {
                // Read the entire file and save its contents.
                string fileContents = File.ReadAllText(saveFile);
                return JsonConvert.DeserializeObject<T>(fileContents);
                // Work with JSON
            }          
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        return data;
    }
}