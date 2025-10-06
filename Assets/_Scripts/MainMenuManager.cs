using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public void PlayButtonPressed()
    {
        SceneLoader.instance.LoadScene("JaredScene");
    }

    public void QuitButtonPressed()
    {
        Application.Quit();
    }
}
