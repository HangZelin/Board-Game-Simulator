using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Write to or read from a new file in Application.persistentDataPath

public class FileManager : MonoBehaviour
{
    public static bool WriteToFile(string fileName, string fileContents)
    {
        // Possible issues on devices of different OS 
        string fullPath = Application.persistentDataPath + "/" + fileName;

        try
        {
            File.WriteAllText(fullPath, fileContents);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write to {fullPath} with exception {e}");
        }

        return false;
    }

    public static bool LoadFromFile(string fileName, out string result)
    {
        string fullPath = Application.persistentDataPath + "/" + fileName;

        try
        {
            result = File.ReadAllText(fullPath);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to read from {fullPath} with exception {e}");
            result = "";
            return false;
        }
    }
}
