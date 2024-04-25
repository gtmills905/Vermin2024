using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerManager : MonoBehaviour
{
    // Dictionary to store player preferences
    public Dictionary<int, GameObject> playerBirdPreferences = new Dictionary<int, GameObject>(); // Player number to selected bird mapping
    public Dictionary<int, GameObject> playerFarmerPreferences = new Dictionary<int, GameObject>(); // Player number to selected farmer mapping
    public Dictionary<int, GameObject> playerCharacterMapping = new Dictionary<int, GameObject>(); // Player number to character mapping

    // Maximum number of players
    public int maxPlayers = 4;

    public List<GameObject> availableFarmers; // List of available farmer prefabs
    public List<GameObject> availableBirds; // List of available bird prefabs

    public void AddPlayer(int playerNumber, GameObject selectedFarmer, GameObject character)
    {
        // Check if the player number is valid and not already assigned
        if (playerNumber > 0 && playerNumber <= maxPlayers && !playerCharacterMapping.ContainsKey(playerNumber))
        {
            // Assign character to the player
            playerCharacterMapping[playerNumber] = character;

            // Set default farmer preference for the player
            playerFarmerPreferences[playerNumber] = selectedFarmer;

            // Randomly select a bird from the availableBirds list
            GameObject selectedBird = GetRandomBirdPrefab();
            if (selectedBird != null)
            {
                // Set the selected bird preference for the player
                playerBirdPreferences[playerNumber] = selectedBird;
            }
            else
            {
                Debug.LogWarning("Failed to add player. No available birds.");
            }

            Debug.Log("Player " + playerNumber + " added with character " + character.name);
        }
        else
        {
            Debug.LogWarning("Failed to add player. Player number already assigned or invalid.");
        }
    }

    private GameObject GetRandomBirdPrefab()
    {
        if (availableBirds.Count > 0)
        {
            int randomIndex = Random.Range(0, availableBirds.Count);
            return availableBirds[randomIndex];
        }
        else
        {
            Debug.LogError("No available bird prefabs.");
            return null;
        }
    }

    public GameObject GetFarmerPreference(int playerNumber)
    {
        if (playerFarmerPreferences.ContainsKey(playerNumber))
        {
            return playerFarmerPreferences[playerNumber];
        }
        return null;
    }

    public int GetPlayerNumberForCharacter(GameObject character)
    {
        foreach (var kvp in playerCharacterMapping)
        {
            // Check if the value associated with the key (player number) is equal to the provided GameObject
            if (kvp.Value == character)
            {
                return kvp.Key; // Return the player number
            }
        }
        return -1; // Return -1 if the character is not found
    }
}
