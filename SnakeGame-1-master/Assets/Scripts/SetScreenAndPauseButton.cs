using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetScreenAndPauseButton : MonoBehaviour
{
    GameObject btnPause;

    private bool paused = false;
    public GameObject dimmer;
    public GameObject pauseText;
    public GameObject txtScore;
    public GameObject txtSlider;

    public Slider slider;

    #region **Old**ScreenSizes
    //int[,] screenSizes = { { 240, 320 }, { 320, 480 }, { 400, 800 }/*5.5*/, { 480, 800 }, { 540, 960 }, { 600, 800 }, { 600, 1024 }, { 640, 960 }, { 640, 1136 }, { 720, 1280 }, { 720, 1440 }/*5.5*/, { 750, 1334 }, { 768, 1024 }, { 768, 1280 }, { 768, 1360 }, { 768, 1366 }, { 800, 1280 }, { 864, 1152 }, { 864, 1536 }, { 900, 1440 }, { 900, 1600 }, { 1024, 1280 }, { 1050, 1680 }, { 1080, 1920 }, { 1080, 2160 }, { 1125, 2436 }/*6*/, { 1200, 1920 }, { 1440, 2560 }, { 1440, 2960 }/*6*/, { 1440, 3440 }/*6.5*/, { 1536, 2048 }, { 2048, 2732 }, { 2160, 3840 } };
    //float[,] ScreenValues = new float[,] { { 240, 320}, { 320, 480}, { 400, 800}, 
    //    { 480, 800}, { 540, 960}, { 600, 800}, { 600, 1024}, 
    //    { 640, 960}, { 640, 1136}, { 720, 1280}, { 720, 1440}, 
    //    { 750, 1334}, { 768, 1024}, { 768, 1280}, { 768, 1360}, 
    //    { 768, 1366}, { 800, 1280 }, { 864, 1152}, { 864, 1536 }, 
    //    { 900, 1440}, { 900, 1600}, { 1024, 1280}, { 1050, 1680}, 
    //    { 1080, 1920}, { 1080, 2160}, { 1125, 2436}, { 1200, 1920}, 
    //    { 1440, 2560}, { 1440, 2960}, { 1440, 3440}, { 1536, 2048}, 
    //    { 2048, 2732}, { 2160, 3840} };


    
    #endregion

    private void Awake()
    {
        slider.value = PlayerPrefs.GetInt("Sensitivity");

        int sw = Screen.width;
        int sh = Screen.height;
        float aspectRatio = (float)sh / sw;

        //1440,3440 | 6.5   1440,2960 | 1125,2436 | 6  400,800 | 720,1440 | 5.5
        // set orthogonal camera size for different aspect ratio
        btnPause = GameObject.Find("btnPause");
        RectTransform rt = btnPause.GetComponent<RectTransform>();
        if ((sw == 720 && sh == 1440) || (sw == 400 && sh == 800) || aspectRatio == 2)
        {
            Camera.main.orthographicSize = 5.5f;
            rt.localPosition = new Vector3(95, 216, 10);
            rt.sizeDelta = new Vector2(85, 85);
        }
        else if ((sw == 1125 && sh == 2436) || (sw == 1440 && sh == 2960))
        {
            Camera.main.orthographicSize = 6;
            rt.localPosition = new Vector3(87, 198, 10);
            rt.sizeDelta = new Vector2(78, 78);
        }
        else if (sw == 1440 && sh == 3440)
        {
            Camera.main.orthographicSize = 6.5f;
            rt.localPosition = new Vector3(80.5f, 182.7f, 10);
            rt.sizeDelta = new Vector2(72, 72);
        }
        else
        {
            Camera.main.orthographicSize = 5;
            rt.localPosition = new Vector3(103.6f, 241, 10);
            rt.sizeDelta = new Vector2(94.72f, 88.16f);
        }



        #region **Old**
        //pause = new GameObject();

        //// set parent to canvas
        //pause.transform.SetParent(GameObject.FindGameObjectWithTag("GameCanvas").transform);
        //// add button component
        //pause.AddComponent<Button>();
        //pause.AddComponent<RectTransform>();
        //pause.AddComponent<Image>();

        //Button btnPause = pause.GetComponent<Button>();
        //RectTransform rt = pause.GetComponent<RectTransform>();
        //image = pause.GetComponent<Image>();

        //// set button image
        //image.sprite = pauseImage;

        //// Get pause button position and size values from screenvalues arr where screen height and width are equal
        //Vector3 btnPos = Vector3.zero;
        //Vector2 btnSize = Vector2.zero;
        //for (int i = 0; i < 33; i++)
        //{
        //    //Debug.Log("Screen Values: " + ScreenValues[i,0]+" "+ ScreenValues[i,1] + " " + ScreenValues[i,2] + " " + ScreenValues[i,3] + " " + ScreenValues[i,4]);
        //    if (sw == ScreenValues[i, 0] && sh == ScreenValues[i, 1])
        //    {
        //        btnPos = new Vector3(ScreenValues[i, 2], ScreenValues[i, 3], 10);
        //        btnSize = new Vector2(ScreenValues[i, 4], ScreenValues[i, 4]);
        //    }
        //}

        //// set RectTransform values
        //rt.localPosition = btnPos;
        ////rt.localPosition = new Vector3(0, 0, 10);
        //rt.sizeDelta = btnSize;
        //rt.localScale = new Vector3(1, 1, 1);

        //// add event listener to button
        //btnPause.onClick.AddListener(Pause); 
        #endregion

    }

    public void Pause()
    {
        if (!paused)
        {
            paused = true;
            Time.timeScale = 0;
            dimmer.SetActive(true);
            pauseText.SetActive(true);
            txtScore.GetComponent<Text>().color = new Color32(160, 160, 160, 255);

            slider.gameObject.SetActive(true);
            txtSlider.SetActive(true);
        }
        else
        {
            UnPause();
        }
    }

    private void UnPause()
    {
        paused = false;
        Time.timeScale = 1;
        dimmer.SetActive(false);
        pauseText.SetActive(false);
        slider.gameObject.SetActive(false);

        txtScore.GetComponent<Text>().color = new Color32(255, 255, 255, 255);
        txtSlider.SetActive(false);
    }

    public void SetSensitivity()
    {
        PlayerPrefs.SetInt("Sensitivity", (int)slider.value);
        Player.minSwipeDistance = (int)slider.value;
    }
}
