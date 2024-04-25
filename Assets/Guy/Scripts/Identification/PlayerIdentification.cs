using UnityEngine;
using TMPro;

public class PlayerIdentification : MonoBehaviour
{
    public PlayerControllerManager playerControllerManager; // Reference to the Player Controller Manager
    public TextMeshProUGUI playerNameText; // Reference to the TextMeshProUGUI component

    void Start()
    {
        // Get player number assigned to this character
        int playerNumber = playerControllerManager.GetPlayerNumberForCharacter(gameObject);

        // Update player name text
        playerNameText.text = "Player " + playerNumber;
    }

    // Method to update player name text
    public void UpdatePlayerName(string playerName)
    {
        playerNameText.text = playerName;
    }

    // Method to destroy player identification prefab
    public void DestroyPlayerIdentification()
    {
        Destroy(gameObject);
    }
}
