using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetStatus : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameStatus.ResetStatus();   
    }
}
