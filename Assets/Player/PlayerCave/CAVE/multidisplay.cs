using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class multidisplay : MonoBehaviour {

    public Camera[] multiDisplay = new Camera[4];
    void Start()
    {
        for (int i = 0; i < Display.displays.Length; i++)
        {
            multiDisplay[i].targetDisplay = i; 
            Display.displays[i].Activate(); 
        }
    }
}
