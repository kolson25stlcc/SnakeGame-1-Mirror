using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeButtonEvents : MonoBehaviour
{

    public void OpenGameScene()
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void OpenOptions()
    {
        SceneManager.LoadScene("Options", LoadSceneMode.Single);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
