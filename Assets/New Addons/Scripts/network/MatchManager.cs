using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Runtime.InteropServices.ComTypes;


public class MatchManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static MatchManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public enum EventCodes
    {
        NewPlayer = 1,
        ListPlayers = 2,
        UpdateStats = 3
    }

    public enum GameState
    {
        Waiting,
        Playing,
        Ending
    }

    public GameManager gameManager;

    public int killsToWin = 10;
    public GameState state = GameState.Waiting;
    public float waitAfterEnding = 5f;

    public List<PlayerInfo> allPlayers = new List<PlayerInfo>();
    private int index;

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("Vermin Menu");
        }
        else
        {
            PhotonNetwork.OpRemoveCompleteCache(); // Clear the cache
            NewPlayerSend(PhotonNetwork.NickName);
            state = GameState.Playing;
        }
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient) // Only the host should check win conditions
        {
            CheckWinConditions();
        }
    }

    private void CheckWinConditions()
    {
        if (gameManager == null) return;

        if (gameManager.birdScore == 10)
        {
            PhotonNetwork.LoadLevel("Birds Win 1");
        }
        else if (gameManager.BirdLives == 10)
        {
            PhotonNetwork.LoadLevel("Farmer Win 1");
        }
        else if (gameManager.timer <= 0f)
        {
            if (gameManager.birdScore + gameManager.BirdLives == 10 || gameManager.birdScore + gameManager.BirdLives == 0)
            {
                PhotonNetwork.LoadLevel("Tie 1");
            }
            else if (gameManager.birdScore > gameManager.BirdLives)
            {
                PhotonNetwork.LoadLevel("Birds Win 1");
            }
            else if (gameManager.BirdLives > gameManager.birdScore || gameManager.BirdLives <= 0)
            {
                PhotonNetwork.LoadLevel("Farmer Win 1");
            }
            else
            {
                PhotonNetwork.LoadLevel("Tie 1");
            }
        }
    }

    public void OnEvent(ExitGames.Client.Photon.EventData photonEvent)
    {
        if (photonEvent.Code < 200) // System event codes
        {
            EventCodes theEvent = (EventCodes)photonEvent.Code;

            if (photonEvent.CustomData is ExitGames.Client.Photon.Hashtable hashtable)
            {
                switch (theEvent)
                {
                    case EventCodes.NewPlayer:

                            NewPlayerReceive(new object[] { hashtable }); // Wrap the hashtable in an object array

                        break;
                    case EventCodes.ListPlayers:

                            ListPlayersReceive(new object[] { hashtable }); // Wrap the hashtable in an object array

                        break;
                    case EventCodes.UpdateStats:

                            UpdateStatsReceive(new object[] { hashtable }); // Wrap the hashtable in an object array

                        break;
                    default:
                        break;
                }
            }
        }

    }






    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void NewPlayerSend(string username)
    {
        object[] package = new object[4];
        package[0] = username;
        package[1] = PhotonNetwork.LocalPlayer.ActorNumber;
        package[2] = 0; // Placeholder for kills
        package[3] = 0; // Placeholder for deaths

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.NewPlayer,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
            new SendOptions { Reliability = true }
        );
    }

    public void NewPlayerReceive(object[] dataReceived)
    {
        if (dataReceived == null || dataReceived.Length != 4)
        {
            Debug.LogError("NewPlayerReceive: Invalid data format.");
            return;
        }

        string username = (string)dataReceived[0];
        int actorNumber = (int)dataReceived[1];
        int kills = (int)dataReceived[2];
        int deaths = (int)dataReceived[3];

        // Add the new player to the list
        PlayerInfo newPlayer = new PlayerInfo(username, actorNumber, kills, deaths, 0); // Deposits initialized to 0
        allPlayers.Add(newPlayer);

        // Log the new player's info
        Debug.Log($"New player added: {username} (Actor: {actorNumber}, Kills: {kills}, Deaths: {deaths})");

        // You can add additional logic here, such as updating the UI or notifying other players
    }


    public void ListPlayersSend()
    {
        object[] package = new object[allPlayers.Count + 1]; // Add 1 for the state
        package[0] = state; // Send game state first

        for (int i = 0; i < allPlayers.Count; i++) // Iterate through allPlayers
        {
            PlayerInfo player = allPlayers[i];
            object[] piece = new object[5]; // Assuming 5 pieces: name, actor, kills, deaths, deposits
            piece[0] = player.name;
            piece[1] = player.actor;
            piece[2] = player.kills;
            piece[3] = player.deaths;
            piece[4] = player.deposits;

            package[i + 1] = piece; // Assign the player's data to the package
        }

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.ListPlayers,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
        );
    }



    public void ListPlayersReceive(object[] dataReceived)
    {
        if (dataReceived == null || dataReceived.Length == 0)
        {
            Debug.LogError("ListPlayersReceive: Received null or empty data.");
            return;
        }

        allPlayers.Clear();

        state = (GameState)dataReceived[0];

        for (int i = 1; i < dataReceived.Length; i++)
        {
            object[] piece = dataReceived[i] as object[];
            if (piece == null || piece.Length != 4)
            {
                Debug.LogError($"Invalid data format at index {i}: Expected object[] of length 4.");
                continue;
            }

            PlayerInfo player = new PlayerInfo(
                    (string)piece[0],
                    (int)piece[1],
                    (int)piece[2],
                    (int)piece[3],
                     (int)piece[4]
                                 );

            

            allPlayers.Add(player);
            if (PhotonNetwork.LocalPlayer.ActorNumber == player.actor)
            {
                index = i - 1;
            }
        }
        StateCheck();
    }

    public void UpdateDepositsSend(int actorSending, int amountToChange)
    {
        object[] package = new object[] { actorSending, amountToChange };

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.UpdateStats,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
        );
    }
    public void UpdateDepositsReceive(object[] dataReceived)
    {
        int actor = (int)dataReceived[0];
        int amount = (int)dataReceived[1];

        bool found = false;
        for (int i = 0; i < allPlayers.Count; i++)
        {
            if (allPlayers[i].actor == actor)
            {
                allPlayers[i].deposits += amount;
                found = true;
                break;
            }
        }

        if (!found)
        {
            Debug.LogWarning($"Player with actor {actor} not found when updating deposits.");
        }
    }



    public void UpdateStatsSend(int actorSending, int statToUpdate, int amountToChange)
    {
        object[] package = new object[] { actorSending, statToUpdate, amountToChange };

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.UpdateStats,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
        );
    }

    public enum StatType
    {
        Kills,
        Deaths,
        Deposits // Future stat type
    }

    public void UpdateStatsReceive(object[] dataReceived)
    {
        int actor = (int)dataReceived[0];
        StatType statType = (StatType)(int)dataReceived[1]; // Cast stat type to enum
        int amount = (int)dataReceived[2];

        for (int i = 0; i < allPlayers.Count; i++)
        {
            if (allPlayers[i].actor == actor)
            {
                switch (statType)
                {
                    case StatType.Kills:
                        allPlayers[i].kills += amount;
                        break;
                    case StatType.Deaths:
                        allPlayers[i].deaths += amount;
                        break;
                        // Add new cases for additional stats
                }
                break;
            }
        }
        ScoreCheck(); // Check for winners after stats update
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        SceneManager.LoadScene("Vermin Menu");
    }
    void ScoreCheck()
    {
        bool winnerFound = false;
        foreach (PlayerInfo player in allPlayers)
        {
            if (player.kills >= killsToWin && killsToWin > 0)
            {
                winnerFound = true;
                break; // Exit loop if winner found
            }
        }

        if (winnerFound && PhotonNetwork.IsMasterClient && state != GameState.Ending)
        {
            state = GameState.Ending; // Set game state to ending
            ListPlayersSend(); // Send updated player states
        }
    }

    void EndGame()
    {
        state = GameState.Ending; // Set game state to ending
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll(); // Clean up all objects in the scene
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(EndCo()); // Start the end coroutine
    }

    void StateCheck()
    {
        if (state == GameState.Ending)
        {
            EndGame(); // Ensure the game ends if the state is "Ending"
        }
    }

    private IEnumerator EndCo()
    {
        yield return new WaitForSeconds(waitAfterEnding);
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LeaveRoom();
    }
}

[System.Serializable]
public class PlayerInfo
{
    public string name;
    public int actor;
    public int kills;
    public int deaths;
    public int deposits; // New field to track deposits


    public PlayerInfo(string _name, int _actor, int _kills, int _deaths, int _deposits)
    {
        name = _name;
        actor = _actor;
        kills = _kills;
        deaths = _deaths;
        deposits = _deposits;
    }

}

