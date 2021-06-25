using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{

    public GameObject dialog;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //捕获Android的系统返回键
        if (Application.platform == RuntimePlatform.Android && (Input.GetKeyDown(KeyCode.Escape)))
        {
            //弹窗，让玩家确认是否退出
            dialog.SetActive(true);
        }
    }

    public void IsQuit(bool quit)
    {
        if (quit)
        {
            Application.Quit();
        }
    }
}