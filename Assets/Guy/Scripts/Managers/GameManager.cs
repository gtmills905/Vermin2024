using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun; // For PUN 2
using Photon.Realtime; // Optional for room/lobby management
using System.Collections.Generic;



public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance; // Static instance field

    public TextMeshProUGUI birdScoreText;
    public TextMeshProUGUI BirdLivesText;
    public TextMeshProUGUI timerText;

    public AudioSource givemebackmypigAudioSource;
    public AudioClip givemebackmypigSoundClip;

    public MatchManager matchManager;  // Reference to MatchManager

    public int playerID; // Or use a property with get/set if needed

    public int birdScore = 0;
    public int BirdLives = 0;

    public DropOffSystem[] dropOffSystem;

    public int depositedAnimalCount = 0;

    public float timer = 300f; // 5 minutes for the first mode

    private void Awake()
    {
        // Ensure only one instance of GameManager exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Another instance of GameManager already exists. Destroying this one.");
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        matchManager = MatchManager.Instance; // Assign the MatchManager instance
        UpdateScoreText(playerID, BirdLives, birdScore);
        UpdateTimerText();
    }
    public PlayerInfo GetPlayerInfo(Player player)
    {
        // Find MatchManager in the scene (assumes there's one instance of MatchManager)
        MatchManager matchManager = FindObjectOfType<MatchManager>();

        if (matchManager != null)
        {
            // Fetch PlayerInfo using the player's actor number (or another unique ID)
            int playerId = player.ActorNumber;  // Photon actor number is typically used

            return matchManager.GetPlayerInfo(playerId);
        }

        return null; // Return null if MatchManager is not found
    }


    public bool IsFarmerPresent()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            // Assuming roles are stored as a custom property or something
            if (player.CustomProperties.ContainsKey("Role") && player.CustomProperties["Role"].ToString() == "Farmer")
            {
                return true;
            }
        }
        return false;
    }


    private void Update()
    {
        timer -= Time.deltaTime;
        UpdateTimerText();
        UpdateScoreText(playerID, BirdLives, birdScore);

        // Send updated scores to MatchManager via Photon
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            MatchManager.Instance.UpdateStatsSend(PhotonNetwork.LocalPlayer.ActorNumber, (int)MatchManager.StatType.Kills, birdScore);
            MatchManager.Instance.UpdateStatsSend(PhotonNetwork.LocalPlayer.ActorNumber, (int)MatchManager.StatType.Deaths, BirdLives);

        }


    }

    // Call this method when a bird deposits an object
    public void DepositObject(int points)
    {
        birdScore += points;
        MatchManager.Instance.UpdateDepositsSend(PhotonNetwork.LocalPlayer.ActorNumber, 1);
        UpdateScoreText(playerID, BirdLives, birdScore);
    }

    // Call this method when the farmer kills a bird
    public void FarmerKill(int points)
    {
        BirdLives += points;  // Increment Farmer's score or kills
        UpdateScoreText(playerID, BirdLives, birdScore);
    }

    public void UpdateScoreText(int playerID, int birdLives, int birdScore)
    {
        // Assuming birdScoreText and BirdLivesText are for the local player's UI
        birdScoreText.text = "     Score: " + birdScore.ToString();
        BirdLivesText.text = "Lives Taken: " + birdLives.ToString();
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public GameObject GetPlayerObject(int playerID)
    {
        // Assuming MatchManager has a dictionary or list that maps player IDs to player objects
        GameObject playerObject = MatchManager.Instance.GetPlayerObjectByID(playerID);

        if (playerObject != null)
        {
            return playerObject;
        }
        else
        {
            Debug.LogWarning("Player object with ID " + playerID + " not found.");
            return null;
        }
    }


    public GameObject GetPlayerPig(int playerID)
    {
        // Get all pigs from the PigManager
        List<GameObject> allPigs = PigManager.GetAllPigs();

        // Iterate through each pig and check if it's owned by the specified player
        foreach (var pig in allPigs)
        {
            if (pig == null)
                continue;  // Skip null pigs

            PhotonView pigView = pig.GetComponent<PhotonView>();
            if (pigView != null && pigView.Owner.ActorNumber == playerID)
            {
                return pig;  // Return the pig if it's owned by the player
            }
        }

        Debug.LogWarning($"No pig found for player with ID {playerID}.");
        return null;  // Return null if no pig is found for the given player ID
    }

    public void SetRole(Player player, string role)
    {
        // Assuming PlayerInfo is the custom player class containing player information
        if (player != null)
        {
            PlayerInfo playerInfo = GetPlayerInfo(player); // Assuming you have a method to get player data
            if (playerInfo != null)
            {
                playerInfo.Role = role; // Set the role for the player
                Debug.Log($"{player.NickName} is now the {role}.");
            }
        }
    }



    public void EndGame()
    {
        if (matchManager != null)
        {
            matchManager.EndGame(); // Call EndGame in MatchManager
        }
        else
        {
            Debug.LogError("MatchManager not found.");
        }
    }



    public int GetNumPlayers()
    {
        return 4;
    }
}

