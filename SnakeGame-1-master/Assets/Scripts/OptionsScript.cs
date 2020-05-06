using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsScript : MonoBehaviour
{
    #region Difficulty
    private int difficulty;
    private int dEasy = (int)GameSettings.Difficulty.Easy;
    private int dMedium = (int)GameSettings.Difficulty.Medium;
    private int dHard = (int)GameSettings.Difficulty.Hard;

        #region DifficultyText
        private Text dText;
        private string tEasy = "Easy";
        private string tMedium = "Medium";
        private string tHard = "Hard";
        private string tNinja = "Ninja";
    #endregion
    #endregion

    private int sensitivity;
    public Slider slider;

    public Canvas mainCanvas;
    public Canvas optionsCanvas;

    private void Start()
    {
        // initialize difficulty text to current setting
        dText = GameObject.Find("btnDifficulty").GetComponentInChildren<Text>();
        difficulty = PlayerPrefs.GetInt("Difficulty");
        if (difficulty == dEasy)
            dText.text = tEasy;
        else if (difficulty == dMedium)
            dText.text = tMedium;
        else if (difficulty == dHard)
            dText.text = tHard;

        slider.value = PlayerPrefs.GetInt("Sensitivity");

    }

    public void SetDifficulty()
    {
        if (difficulty == dEasy)
        {
            difficulty = dMedium;
            PlayerPrefs.SetInt("Difficulty", dMedium);
            dText.text = tMedium;
        }
        else if (difficulty == dMedium)
        {
            difficulty = dHard;
            PlayerPrefs.SetInt("Difficulty", dHard);
            dText.text = tHard;
        }
        else if (difficulty == dHard)
        {
            difficulty = dEasy;
            PlayerPrefs.SetInt("Difficulty", dEasy);
            dText.text = tEasy;
        }
        //Debug.Log("Difficulty Level: " + PlayerPrefs.GetInt("Difficulty"));
    } 

    public void SetSensitivity()
    {
        PlayerPrefs.SetInt("Sensitivity", (int)slider.value);
    }

    public void OpenOptions()
    {
        optionsCanvas.gameObject.SetActive(true);
        mainCanvas.gameObject.SetActive(false);
    }

    public void CloseOptions()
    {
        optionsCanvas.gameObject.SetActive(false);
        mainCanvas.gameObject.SetActive(true);
    }
}
