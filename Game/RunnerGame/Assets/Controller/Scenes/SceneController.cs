using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

/// <summary>
/// Loads scenes and exits the application.
/// </summary>
public class SceneController : MonoBehaviour {

    /// <summary>
    /// Loads a scene by its registered name.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    public void LoadByName(String sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Loads a scene by its index in the build settings.
    /// </summary>
    /// <param name="sceneIndex">The build-settings index of the scene to load.</param>
    public void LoadByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    /// <summary>
    /// Requests that the application exit.
    /// </summary>
    public void QuitProgramm()
    {
        Application.Quit();
    }

}
