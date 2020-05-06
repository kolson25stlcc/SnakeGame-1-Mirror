using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializePlayerPrefs : MonoBehaviour
{

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("Difficulty"))
            PlayerPrefs.SetInt("Difficulty", (int)GameSettings.Difficulty.Easy);
        if (!PlayerPrefs.HasKey("ControlType"))
            PlayerPrefs.SetInt("ControlType", (int)GameSettings.ControlType.Swipe);
        if (!PlayerPrefs.HasKey("SnakeType"))
            PlayerPrefs.SetInt("SnakeType", (int)GameSettings.SnakeType.Original);
        if (!PlayerPrefs.HasKey("Sensitivity"))
            PlayerPrefs.SetInt("Sensitivity", 75);
        

    }

}
