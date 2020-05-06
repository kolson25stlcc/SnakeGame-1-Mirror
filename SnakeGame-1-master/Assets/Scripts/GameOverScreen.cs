using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    private ScoreKeeper scoreKeep;
    private ScoreDBController scoreDBController;

    private int highScore;

    //private float startTime = 0;
    //private float tapTime = 1f;

    //private bool tapping = false;
    //private int taps = 0;


    private void Start()
    {
        scoreDBController = Camera.main.GetComponent<ScoreDBController>();
        highScore = scoreDBController.GetHighScore((GameSettings.Difficulty)PlayerPrefs.GetInt("Difficulty"));
        GameObject.Find("txtHighScore").GetComponent<Text>().text = highScore.ToString();
        Destroy(scoreDBController);

        scoreKeep = GameObject.Find("ScoreKeeper").GetComponent<ScoreKeeper>();
        GameObject.Find("txtScoreTotal").GetComponent<Text>().text = scoreKeep.pubScore.ToString();
    }

   
    public void ToMainMenu()
    {
        SceneManager.LoadScene("StartScreen", LoadSceneMode.Single);
    }

    public void Replay()
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    // Update is called once per frame
    void Update()
    {

        //if (Input.GetMouseButtonDown(0))
        //if (Input.GetTouch(0).phase == TouchPhase.Ended)
        //{
        //    tapping = true;
        //    if (taps == 0)
        //    {
        //        taps++;
        //        startTime = Time.time;
        //    }
        //    else if (taps == 1)
        //    {
        //        if (Time.time - startTime < tapTime)
        //        {
        //            SceneManager.LoadScene("StartScreen", LoadSceneMode.Single);
        //        }
        //        else
        //        {
        //            taps = 0;
        //            tapping = false;
        //        }
        //    }
            
        //}

    }
}
