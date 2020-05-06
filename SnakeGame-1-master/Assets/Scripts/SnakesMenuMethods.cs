using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SnakesMenuMethods : MonoBehaviour
{
    public Canvas mainCanvas;
    public Canvas snakesCanvas;

    private GameSettings.SnakeType snakeType;

    private Color32 dimmedColor = new Color32(166, 166, 166, 255);
    private Color32 selectedColor = new Color32(255, 255, 255, 255);

    private void Start()
    {
        
        
        
    }

    private void Update()
    {
        if (snakesCanvas.isActiveAndEnabled)
            glowRGBBtn();
    }

    public void OpenMenu()
    {
        mainCanvas.gameObject.SetActive(false);
        snakesCanvas.gameObject.SetActive(true);

        snakeType = (GameSettings.SnakeType)PlayerPrefs.GetInt("SnakeType");
        string btnName = "";
        if (snakeType == GameSettings.SnakeType.Original)
            btnName = "btnOriginal";
        else if (snakeType == GameSettings.SnakeType.Rainbow)
            btnName = "btnRainbow";
        else if (snakeType == GameSettings.SnakeType.Pastel)
            btnName = "btnPastel";
        else if (snakeType == GameSettings.SnakeType.RGB)
        {
            rgbSelected = true;
            btnName = "btnRGB";
        }

        SetButtons(btnName);

    }

    public void CloseMenu()
    {
        mainCanvas.gameObject.SetActive(true);
        snakesCanvas.gameObject.SetActive(false);
    }

    public void btnOriginal()
    {
        string btnName = "btnOriginal";
        PlayerPrefs.SetInt("SnakeType", (int)GameSettings.SnakeType.Original);

        SetButtons(btnName);
    }

    public void btnRainbow()
    {
        string btnName = "btnRainbow";
        PlayerPrefs.SetInt("SnakeType", (int)GameSettings.SnakeType.Rainbow);

        SetButtons(btnName);
    }
    
    public void btnPastel()
    {
        string btnName = "btnPastel";
        PlayerPrefs.SetInt("SnakeType", (int)GameSettings.SnakeType.Pastel);

        SetButtons(btnName);
    }

    public void btnRGB()
    {
        string btnName = "btnRGB";
        PlayerPrefs.SetInt("SnakeType", (int)GameSettings.SnakeType.RGB);

        SetButtons(btnName);

    }

    private void SetButtons(string selectedButtonName)
    {
        foreach(Image img in snakesCanvas.GetComponentsInChildren<Image>())
        {
            if (img.gameObject.name == selectedButtonName && selectedButtonName != "btnRGB")
            {
                img.color = selectedColor;
                rgbSelected = false;
            }
            else if (img.gameObject.name == "btnBack")
                img.color = selectedColor;
            else if (img.gameObject.name != "btnRGB")
                img.color = dimmedColor;
            else if (selectedButtonName == "btnRGB")
            {
                rgbSelected = true;
            }
        }
    }

    private bool rgbSelected = false;
    private Color32 clr4RGB = new Color32(255, 0, 0, 255);
    private int rateOfChange = 150;
    private void glowRGBBtn()
    {
        int R = clr4RGB.r; int G = clr4RGB.g; int B = clr4RGB.b;
        int changeBy = Mathf.RoundToInt(rateOfChange * Time.deltaTime);

        int bound = 255;

        int alpha;

        if (!rgbSelected)
        {
            alpha = 100;
        }
        else
        {
            alpha = 255;
        }

        if (R == bound && G < bound && B == 0) // add to G til G == 255
        {
            G += changeBy;
            if (G > 255)
                G = 255;
        }
        else if (R > 0 && G == bound && B == 0) // subtract from R until R == 0
        {
            R -= changeBy;
            if (R < 0)
                R = 0;
        }
        else if (R == 0 && G == bound && B < bound) // add to B until B == 255 
        {
            B += changeBy;
            if (B > 255)
                B = 255;
        }
        else if (R == 0 && G > 0 && B == bound) // subtract from G until G == 0
        {
            G -= changeBy;
            if (G < 0)
                G = 0;
        }
        else if (R < bound && G == 0 && B == bound) // add to R until R == 255
        {
            R += changeBy;
            if (R > 255)
                R = 255;
        }
        else if (R == bound && G == 0 && B > 0) // subtract from B until B == 0
        {
            B -= changeBy;
            if (B < 0)
                B = 0;
        }
        clr4RGB = new Color32((byte)R, (byte)G, (byte)B, (byte)alpha);

        GameObject.Find("btnRGB").GetComponent<Image>().color = clr4RGB;
    }
}
