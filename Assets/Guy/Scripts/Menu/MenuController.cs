using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public PlayerControllerManager playerControllerManager; // Reference to the Player Controller Manager
    public List<GameObject> birdPrefabs; // List of bird prefabs
    public List<GameObject> farmerPrefabs; // List of farmer prefabs
    public Transform spawnPosition;
    public int maxPlayers = 4; // Declare maxPlayers
    private int playersJoined = 0; // Track the number of players joined

    private bool farmerAlreadySelected = false; // Track if a farmer has been selected

    void Start()
    {
        LoadPlayerPreferences(); // Load player preferences on start
    }

    public void AddPlayer(int playerNumber, GameObject selectedBird, GameObject selectedFarmer, GameObject character)
    {
        // Check if the player number is valid and not already assigned
        if (playerNumber > 0 && playerNumber <= maxPlayers && !playerControllerManager.playerCharacterMapping.ContainsKey(playerNumber))
        {
            // Increment the count of players joined
            playersJoined++;

            // Set default bird and farmer preferences for the player
            playerControllerManager.playerBirdPreferences[playerNumber] = selectedBird;
            playerControllerManager.playerFarmerPreferences[playerNumber] = selectedFarmer;

            // Instantiate bird prefab and initialize with player number
            GameObject selectedBirdPrefab = GetRandomBirdPrefab(); // Get a random bird prefab

            // Check if all players have joined
            if (playersJoined == maxPlayers)
            {
                StartGameScene();
            }
        }
        else
        {
            Debug.LogWarning("Failed to add player. Player number already assigned or invalid.");
        }
    }

    private void StartGameScene()
    {
        SceneManager.LoadScene("Vermin");
    }

    private GameObject GetRandomBirdPrefab()
    {
        if (birdPrefabs.Count > 0)
        {
            int randomIndex = Random.Range(0, birdPrefabs.Count);
            return birdPrefabs[randomIndex];
        }
        else
        {
            Debug.LogError("No bird prefabs available.");
            return null;
        }
    }

    // Load player preferences from PlayerPrefs
    private void LoadPlayerPreferences()
    {
        for (int i = 1; i <= maxPlayers; i++)
        {
            string birdPrefKey = "BirdPreference_Player_" + i;
            string farmerPrefKey = "FarmerPreference_Player_" + i;

            if (PlayerPrefs.HasKey(birdPrefKey))
            {
                string birdPrefabPath = PlayerPrefs.GetString(birdPrefKey);
                GameObject birdPrefab = Resources.Load<GameObject>(birdPrefabPath);
                playerControllerManager.playerBirdPreferences[i] = birdPrefab;
            }

            if (PlayerPrefs.HasKey(farmerPrefKey))
            {
                string farmerPrefabPath = PlayerPrefs.GetString(farmerPrefKey);
                GameObject farmerPrefab = Resources.Load<GameObject>(farmerPrefabPath);
                playerControllerManager.playerFarmerPreferences[i] = farmerPrefab;
                farmerAlreadySelected = true; // Mark farmer as selected
            }
        }
    }

    // Save player preferences to PlayerPrefs
    private void SavePlayerPreferences(List<GameObject> selectedBirds, List<GameObject> selectedFarmers, int playerNumber)
    {
        if (selectedBirds != null && selectedFarmers != null)
        {
            // Serialize the selected bird and farmer prefabs to JSON strings
            string birdJson = JsonUtility.ToJson(selectedBirds);
            string farmerJson = JsonUtility.ToJson(selectedFarmers);

            PlayerPrefs.SetString($"BirdPreference_Player_{playerNumber}", birdJson);
            PlayerPrefs.SetString($"FarmerPreference_Player_{playerNumber}", farmerJson);
        }
        else
        {
            Debug.LogError("Either the selected bird or farmer list is null.");
        }
    }
}
