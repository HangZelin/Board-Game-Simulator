using System;
using System.IO;
using UnityEngine;

/// <Summary> Write to or read from a new file in Application.persistentDataPath. </Summary>

public class FileManager : MonoBehaviour
{
    /** <summary>
     * Create/Overwrite a file in Application.persistentDataPath
     * </summary>
     * <return> A bool that indicates success or not </return>
     * <param name="fileName"> Name of file to create/edit </param>
     * <param name="fileContents"> Contents of the file  </param>
     */
    public static bool WriteToFile(string fileName, string fileContents)
    {
        string fullPath = Path.Combine(Application.persistentDataPath, fileName);

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

    /** <summary>
     * Load from a file in Application.persistentDataPath, then output a string.
     * </summary>
     * <return> A bool that indicates success or not </return>
     * <param name="fileName"> Name of file to create/edit </param>
     * <param name="result"> Contents of the file as an output string. </param>
     */
    public static bool LoadFromFile(string fileName, out string result)
    {
        string fullPath = Path.Combine(Application.persistentDataPath, fileName);

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
