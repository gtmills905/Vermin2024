using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Runtime.InteropServices;

public class MatchManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static MatchManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public enum EventCodes : byte
    {
        NewPlayer,
        ListPlayers,
        UpdateStats
    }

    public List<PlayerInfo> allPlayers = new List<PlayerInfo>();
    private int index;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnEvent(EventData photonEvent)
    {
        // Make sure we're handling events below 200 (custom events)
        if (photonEvent.Code < 200)
        {
            EventCodes theEvent = (EventCodes)photonEvent.Code;

            // Log the event data type for debugging purposes
            Debug.Log($"OnEvent: Received data for event code {theEvent}, data type: {photonEvent.CustomData.GetType()}");

            // Check the type of CustomData before casting
            if (photonEvent.CustomData is object[] data)
            {
                // Safely proceed with event handling
                switch (theEvent)
                {
                    case EventCodes.NewPlayer:
                        NewPlayerReceive(data);
                        break;

                    case EventCodes.ListPlayers:
                        ListPlayersReceive(data);
                        break;

                    case EventCodes.UpdateStats:
                        UpdateStatsReceive(data);
                        break;

                    default:
                        Debug.LogWarning($"OnEvent: Unhandled event code {theEvent}");
                        break;
                }
            }
            else
            {
                // Log the unexpected data type
                Debug.LogError($"OnEvent: Unexpected data type for event code {theEvent}. Expected object[], but got {photonEvent.CustomData.GetType()}");
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
        // Ensure to send an object array
        object[] package = new object[4];
        package[0] = username; // Player's username
        package[1] = PhotonNetwork.LocalPlayer.ActorNumber; // Actor number
        package[2] = 0; // Placeholder for kills
        package[3] = 0; // Placeholder for deaths

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.NewPlayer,
            package, // Send the object array
            new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
            new SendOptions { Reliability = true }
        );
    }


    public void NewPlayerReceive(object[] dataReceived)
    {
        if (dataReceived.Length != 4)
        {
            Debug.LogError($"Invalid data received. Expected 4 elements, got {dataReceived.Length}");
            return;
        }

        string username = dataReceived[0] as string;
        int actorNumber = (int)dataReceived[1];
        int kills = (int)dataReceived[2];
        int deaths = (int)dataReceived[3];

        // Now you can process the data as needed
        Debug.Log($"New Player: {username}, ActorNumber: {actorNumber}, Kills: {kills}, Deaths: {deaths}");
    }

    // Send the "ListPlayers" event
    public void ListPlayersSend()
    {
        object[] package = new object[allPlayers.Count];
        for (int i = 0; i < allPlayers.Count; i++)
        {
            package[i] = allPlayers[i]; // Package all player info
        }

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.ListPlayers,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
        );
    }

    // Receive the "ListPlayers" event
    public void ListPlayersReceive(object[] dataReceived)
    {
        if (dataReceived.Length < 1)
        {
            Debug.LogError($"ListPlayersReceive: Invalid data length. Expected at least 1 player, got {dataReceived.Length}");
            return;
        }

        allPlayers.Clear(); // Clear the current player list before updating

        foreach (var playerData in dataReceived)
        {
            if (playerData is PlayerInfo playerInfo)
            {
                allPlayers.Add(playerInfo); // Add each player info to the list
            }
            else
            {
                Debug.LogError($"ListPlayersReceive: Unexpected data type {playerData.GetType()}");
            }
        }
    }

    // Send the "UpdateStats" event
    public void UpdateStatsSend(int actor, int kills, int deaths)
    {
        object[] package = new object[3];
        package[0] = actor;  // Actor number
        package[1] = kills;  // Kills count
        package[2] = deaths; // Deaths count

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.UpdateStats,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
        );
    }

    // Receive the "UpdateStats" event
    public void UpdateStatsReceive(object[] dataReceived)
    {
        if (dataReceived.Length < 3)
        {
            Debug.LogError($"UpdateStatsReceive: Invalid data length. Expected 3, got {dataReceived.Length}");
            return;
        }

        if (!(dataReceived[0] is int) || !(dataReceived[1] is int) || !(dataReceived[2] is int))
        {
            Debug.LogError($"UpdateStatsReceive: Unexpected data types. Data: {string.Join(", ", dataReceived)}");
            return;
        }

        int actor = (int)dataReceived[0];
        int kills = (int)dataReceived[1];
        int deaths = (int)dataReceived[2];

        // Update the player stats in allPlayers based on the actor number
        PlayerInfo player = allPlayers.Find(p => p.actor == actor);
        if (player != null)
        {
            player.kills = kills;
            player.deaths = deaths;
        }
        else
        {
            Debug.LogError($"UpdateStatsReceive: Player with actor {actor} not found.");
        }
    }
}

[System.Serializable]
public class PlayerInfo
{
    public string name;
    public int actor;
    public int kills;
    public int deaths;

    public PlayerInfo(string _name, int _actor, int _kills, int _deaths)
    {
        name = _name;
        actor = _actor;
        kills = _kills;
        deaths = _deaths;
    }
}
