using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuScript : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Vermin");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Back2Game()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

