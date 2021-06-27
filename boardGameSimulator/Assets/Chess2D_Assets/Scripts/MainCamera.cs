using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainCamera : MonoBehaviour
{
    public float PhoneWidth;
    public float PhoneHeight = Screen.height;
    public float RateOfPhone;
    public float MyWidth = 393;
    public float MyHeight = 851;
    public float RateOfDesine;
    public float TSize = 5f;
    public void FitCamera(Camera camera)
    {
        PhoneWidth = Screen.width;
        PhoneHeight = Screen.height;
        RateOfDesine = MyHeight / MyWidth;
        RateOfPhone = PhoneHeight / PhoneWidth;
        if (RateOfPhone == RateOfDesine)
            camera.orthographicSize = TSize;
        else if (RateOfPhone > RateOfDesine)
            camera.orthographicSize = PhoneHeight * TSize / MyHeight;
        else if (RateOfPhone > RateOfDesine)
            camera.orthographicSize = PhoneWidth * TSize / MyWidth;

    }
    private void Awake()
    {
     

        FitCamera(transform.GetComponent<Camera>());
    }
}