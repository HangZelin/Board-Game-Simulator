using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveBarUI : MonoBehaviour
{
    private SaveData sd;

    public SaveBarUI(int num)
    {
        try
        {
            FileManager.LoadFromFile("SaveData" + num + ".dat", out var json);
            sd.LoadFromJson(json);
        }
        catch (Exception e)
        {
            Debug.LogError("SaveBar: Failed to load from file SaveData" + num + ".dat : " + e);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

}
