using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

/**
 * Controlls the changes between Scenes
 */
public class SceneController : MonoBehaviour {

    public void LoadByName(String sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void QuitProgramm()
    {
        Application.Quit();
    }

}
