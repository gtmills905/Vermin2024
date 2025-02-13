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
        if(!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("Vermin Menu");
        }
        else
        {
            NewPlayerSend(PhotonNetwork.NickName);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code < 200)
        {
            EventCodes theEvent = (EventCodes)photonEvent.Code;
            object[] data = (object[])photonEvent.CustomData;

            switch(theEvent)
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
        package[2] = 0;
        package[3] = 0;

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.NewPlayer, package, new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
            new SendOptions { Reliability = true });
    
    }
    public void NewPlayerReceive(object[] dataRecieved)
    {
        PlayerInfo player = new PlayerInfo((string) dataRecieved[0], (int) dataRecieved[1], (int) dataRecieved[2], (int) dataRecieved[3]);

        allPlayers.Add(player);
    }
    public void ListPlayersSend() 
    {
    
    
    }
    public void ListPlayersReceive(object[] dataRecieved)
    {

    }
    public void UpdateStatsSend()
    {

    }
    public void UpdateStatsReceive(object[] dataRecieved)
    {

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