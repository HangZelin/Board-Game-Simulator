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
        //����Android��ϵͳ���ؼ�
        if (Application.platform == RuntimePlatform.Android && (Input.GetKeyDown(KeyCode.Escape)))
        {
            //�����������ȷ���Ƿ��˳�
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