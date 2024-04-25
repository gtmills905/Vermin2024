using UnityEngine;

public class PlayerIdentificationManager : MonoBehaviour
{
    public PlayerControllerManager playerControllerManager; // Reference to the Player Controller Manager
    public GameObject playerIdentificationPrefab; // Reference to the Player Identification prefab
    public Transform spawnPosition; // Position to spawn the player identifications

    void Start()
    {
        // Instantiate player identifications for each player
        for (int i = 1; i <= playerControllerManager.maxPlayers; i++)
        {
            GameObject playerIdentification = Instantiate(playerIdentificationPrefab, spawnPosition.position, Quaternion.identity);
            PlayerIdentification playerIdentificationScript = playerIdentification.GetComponent<PlayerIdentification>();

            // Set playerControllerManager reference
            playerIdentificationScript.playerControllerManager = playerControllerManager;

            // Set the parent of the player identification to keep the hierarchy clean
            playerIdentification.transform.parent = transform;

            // Call method to update player name text
            playerIdentificationScript.UpdatePlayerName("Player " + i);
        }
    }
}
