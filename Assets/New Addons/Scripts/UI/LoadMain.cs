using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;


public class LoadMain : MonoBehaviourPunCallbacks
{
    // Method to quit to the main menu
    public void QuitToMenu()
    {
        Debug.Log("Quitting to menu...");

        // Unlock the cursor before loading the menu
        UnlockCursor();

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
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Disconnected from Photon: {cause}");
        // Load the menu scene after the disconnection is complete
        LoadMenuScene();
    }

    private void LoadMenuScene()
    {
        // Load the menu scene
        SceneManager.LoadScene("Vermin Menu");
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
