using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using Microsoft.Win32.SafeHandles;
using System.Security.Permissions;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;

    public GameObject loadingScreen;
    public TMP_Text loadingText;

    public GameObject menuButtons;
    public GameObject createRoomScreen;
    public TMP_InputField roomNameInput;

    public GameObject roomScreen;
    public TMP_Text roomNameText, playerNameLabel;
    private List<TMP_Text> allPlayerNames = new List<TMP_Text>();

    public GameObject errorScreen;
    public TMP_Text errorText;

    public GameObject roomBrowserScreen;
    public RoomButton theRoomButton;
    private List<RoomButton> allRoomButtons = new List<RoomButton>();

    public GameObject nameInputScreen;
    public TMP_InputField nameInput;
    private bool hasSetNick;

    public GameObject startButton;

    public string levelToPlay;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        CloseMenus();

        loadingScreen.SetActive(true);
        loadingText.text = "Connecting to Network...";

        PhotonNetwork.ConnectUsingSettings();
    }

    void CloseMenus()
    {
        if (loadingScreen) loadingScreen.SetActive(false);
        if (menuButtons) menuButtons.SetActive(false);
        if (createRoomScreen) createRoomScreen.SetActive(false);
        roomScreen.SetActive(false);
        errorScreen.SetActive(false);
        roomBrowserScreen.SetActive(false);
        nameInputScreen.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();

        PhotonNetwork.AutomaticallySyncScene = true;
        loadingText.text = "Joining Lobby...";
    }

    public override void OnJoinedLobby()
    {
        CloseMenus();
        menuButtons.SetActive(true);

        PhotonNetwork.NickName = Random.Range(0,1000).ToString();
        if(!hasSetNick)
        {
            CloseMenus();
            nameInputScreen.SetActive(true) ;
            if (PlayerPrefs.HasKey("playerName"))
            {
                nameInput.text = PlayerPrefs.GetString("playerName");
            }
        }
        else
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString("playerName");
        }
    }

    public void OpenRoomCreate()
    {
        CloseMenus();
        if (createRoomScreen) createRoomScreen.SetActive(true);
    }

    public void CreateRoom()
    {
        if (!string.IsNullOrEmpty(roomNameInput.text))
        {
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 4;

            
            PhotonNetwork.CreateRoom(roomNameInput.text);
            
            CloseMenus();
            loadingText.text = "Creating Room...";
            loadingScreen.SetActive(true);
        }
    }
    public override void OnJoinedRoom()
    {
        CloseMenus();
        roomScreen.SetActive(true);
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        ListAllPlayers();

        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
        }
    }
    private void ListAllPlayers()
    {
        foreach(TMP_Text player in allPlayerNames)
        {
            Destroy(player.gameObject);
        }
        allPlayerNames.Clear();

        Player[] players = PhotonNetwork.PlayerList;

        for(int i = 0; i < players.Length; i++)
        {
            TMP_Text newPlayerLabel = Instantiate(playerNameLabel, playerNameLabel.transform.parent);
            newPlayerLabel.text = players[i].NickName;
            newPlayerLabel.gameObject.SetActive(true);
            
            allPlayerNames.Add(newPlayerLabel);
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        TMP_Text newPlayerLabel = Instantiate(playerNameLabel, playerNameLabel.transform.parent);
        newPlayerLabel.text = newPlayer.NickName;
        newPlayerLabel.gameObject.SetActive(true);

        allPlayerNames.Add(newPlayerLabel);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ListAllPlayers();
    }
    

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room creation failed: " + message;
        CloseMenus();
        errorScreen.SetActive(true);
    }
    public void CloseErrorScreen()
    {
        CloseMenus();
        menuButtons.SetActive(true);
    }

    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        loadingText.text = "Disconnected: " + cause;
        menuButtons.SetActive(true);
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        CloseMenus();
        loadingText.text = "Leaving Room";
        loadingScreen.SetActive(true);
    }
    public override void OnLeftRoom()
    {
        CloseMenus();
        menuButtons.SetActive(true);
    }
    public void OpenRoomBrowser()
    {
        CloseMenus();
        roomBrowserScreen.SetActive(true);
    }
    public void CloseRoomBrowser()
    {
        CloseMenus();
        menuButtons.SetActive(true);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomlist)
    {
        if (allRoomButtons == null)
        {
            Debug.LogError("allRoomButtons list is not initialized.");
            return;
        }

        foreach (RoomButton rb in allRoomButtons)
        {
            if (rb != null && rb.gameObject != null)
            {
                Destroy(rb.gameObject);
            }
        }
        allRoomButtons.Clear();

        if (theRoomButton == null)
        {
            Debug.LogError("theRoomButton is not assigned.");
            return;
        }

        theRoomButton.gameObject.SetActive(false);

        for (int i = 0; i < roomlist.Count; i++)
        {
            RoomInfo room = roomlist[i];
            if (room == null)
            {
                Debug.LogWarning($"Room at index {i} is null.");
                continue;
            }

            if (room.PlayerCount != room.MaxPlayers && !room.RemovedFromList)
            {
                RoomButton newButton = Instantiate(theRoomButton, theRoomButton.transform.parent);
                if (newButton != null)
                {
                    newButton.SetButtonDetails(room);
                    newButton.gameObject.SetActive(true);
                    allRoomButtons.Add(newButton);
                }
                else
                {
                    Debug.LogError("Failed to instantiate RoomButton.");
                }
            }
        }
    }

    public void JoinRoom(RoomInfo inputInfo)
    {
        PhotonNetwork.JoinRoom(inputInfo.Name);

        CloseMenus();
        loadingText.text = "Joining Room";
        loadingScreen.SetActive(true);
    }

    public void SetNickname()
    {
        if (!string.IsNullOrEmpty(nameInput.text))
        {
            PhotonNetwork.NickName = nameInput.text;

            PlayerPrefs.SetString("playerName", nameInput.text);
            CloseMenus();
            menuButtons.SetActive(true);
            hasSetNick = true;
        }
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
        }
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(levelToPlay);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}