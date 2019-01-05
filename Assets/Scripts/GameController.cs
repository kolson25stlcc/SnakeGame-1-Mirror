using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    private Vector2 onesPlacePos;
    private Vector2 tensPlacePos;
    private Vector2 hundredsPlacePos;

    private int onesPlace = 0;
    private int tensPlace = 0;
    private int hundredsPlace = 0;

    public GameObject zero;
    public GameObject one;
    public GameObject two;
    public GameObject three;
    public GameObject four;
    public GameObject five;
    public GameObject six;
    public GameObject seven;
    public GameObject eight;
    public GameObject nine;

    //public Text scoreText;
    private int score = 0;

    private void Start()
    {
        score = 0;
        hundredsPlace = 0;
        tensPlace = 0;
        onesPlace = 0;

        Debug.Log("Score at start: " + score);
        
        //scoreText.text = "Score: 0";
    }

    public void AddScore ()
    {
        score++;
        UpdateScore();
    }

    private void UpdateScore()
    {
        //scoreText.text = "Score: " + score;
        //Debug.Log("Score before update: " + score);

        if (score >= 100)
        {
            hundredsPlace = getHundredsPlace(score);
        }
        if (score >= 10)
        {
            tensPlace = getTensPlace(score);
        }
        if (score >= 0)
        {
            onesPlace = getOnesPlace(score);
        }

        Debug.Log("Score: " + score + " Calc'd score: " + hundredsPlace + tensPlace + onesPlace);
    }
    private int getHundredsPlace(int num)
    {
        int counter = 0;
        while (num >= 100)
        {
            num -= 100;
            counter++;
        }
        return counter;
    }
    private int getTensPlace(int num)
    {
        num %= 100;
        int counter = 0;
        while (num >= 10)
        {
            num -= 10;
            counter++;
        }
        return counter;
    }
    private int getOnesPlace(int num)
    {
        num %= 100;
        num %= 10;
        int counter = 0;
        while (num >= 1)
        {
            num--;
            counter++;
        }
        return counter;
    }

    public void ResetScore()
    {
        score = 0;
        hundredsPlace = 0;
        tensPlace = 0;
        onesPlace = 0;
    }

    private void OnApplicationQuit()
    {
        ResetScore();
    }
}
