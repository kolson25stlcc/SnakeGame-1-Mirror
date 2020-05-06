using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreKeeper : MonoBehaviour
{
    private int score = 0;
    public int pubScore = 0;

    public void AddScore()
    {
        score++;
        pubScore++;
    }

    private void Start()
    {
        SceneManager.activeSceneChanged += SceneChangedToGameOver;
    }

    private void SceneChangedToGameOver(Scene oldScene, Scene newScene)
    {
        if (newScene.name == "GameOver")
        {
            SaveScore saver = Camera.main.GetComponent<SaveScore>();
            bool[] asdf = saver.SaveIfHighScore(score, (GameSettings.Difficulty)PlayerPrefs.GetInt("Difficulty"));
            //Debug.Log("Worked " + asdf[0] + " New highscore " + asdf[1]); 
        }
        else if (newScene.name == "Game")
        {
            score = 0;
            pubScore = 0;
        }
    }
}
