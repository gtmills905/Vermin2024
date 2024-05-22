using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConInput : MonoBehaviour
{
    // Called when we click the "Play" button.
    public void OnPlayButton()
    {
        SceneManager.LoadScene("Vermin");
    }
    // Called when we click the "Quit" button.
    public void OnQuitButton()
    {
        Application.Quit();
    }
}
