using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class PauseMenu : MonoBehaviourPunCallbacks
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

        // Destroy the MainMenuCanvas if the scene is "Vermin"
        if (SceneManager.GetActiveScene().name == "Vermin")
        {
            GameObject mainMenuCanvas = GameObject.FindGameObjectWithTag("MainMenuCanvas");
            if (mainMenuCanvas != null)
            {
                Destroy(mainMenuCanvas);
            }
        }
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

    public void QuitToMenu()
    {
        Debug.Log("Quitting to menu...");

        // Unlock the cursor before loading the menu
        UnlockCursor();

        // Destroy the player's game object before disconnecting
        if (PhotonNetwork.LocalPlayer.TagObject != null)
        {
            GameObject playerObject = (GameObject)PhotonNetwork.LocalPlayer.TagObject;

            // Ensure the player object has a PhotonView attached for network sync
            if (playerObject.GetComponent<PhotonView>() != null)
            {
                Debug.Log("Destroying local player's game object: " + playerObject.name);
                PhotonNetwork.Destroy(playerObject);  // Destroys the game object for all players
            }
            else
            {
                Debug.LogError("The player's game object does not have a PhotonView attached!");
            }
        }

        // Then disconnect from Photon if necessary
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();  // Disconnect from Photon
        }
        else
        {
            LoadMenuScene();  // If not connected, load the menu scene directly
        }
    }


    // Callback for when the Photon client has disconnected
    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        // Load the menu scene after the disconnection is complete
        LoadMenuScene();
    }

    private void LoadMenuScene()
    {
        // Load the menu scene
        SceneManager.LoadScene("Vermin Menu");
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
