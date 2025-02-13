using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using System.Collections;



public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuCanvas;
    public Button resumeButton;
    public Button quitButton;

    private bool isPaused = false;

    void Start()
    {
        // Ensure the menu is hidden at the start
        pauseMenuCanvas.SetActive(false);

        // Lock the cursor at the start
        LockCursor();

        // Add listeners to buttons
        resumeButton.onClick.AddListener(ResumeGame);
        quitButton.onClick.AddListener(QuitToMenu);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    void TogglePauseMenu()
    {
        isPaused = !isPaused;
        pauseMenuCanvas.SetActive(isPaused);

        if (isPaused)
        {
            UnlockCursor(); // Show the cursor when paused
        }
        else
        {
            LockCursor(); // Hide and lock the cursor when resuming
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuCanvas.SetActive(false);
        LockCursor();

    }

    // Method to quit to the main menu
    public void QuitToMenu()
    {
        Debug.Log("Quitting to menu...");

        // Unlock the cursor before loading the menu
        UnlockCursor();

        

        // Then disconnect from Photon if necessary
        if (PhotonNetwork.IsConnected)
        {
            // Load the menu scene first
            SceneManager.LoadScene("Vermin Menu");
            
        }
    }






    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
