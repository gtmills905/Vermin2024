using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using Microsoft.Win32.SafeHandles;
using System.Security.Permissions;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine.SceneManagement;


public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;

    public static GameManager Instance;

    public GameObject loadingScreen;
    public TMP_Text loadingText;
    public GameObject title;
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
    }

    void Start()
    {
        CloseMenus();
        PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout = 10000; // 10 seconds
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 15;
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "";
        


        loadingScreen.SetActive(true);
        loadingText.text = "Connecting to Network...";


        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Reconnecting...");
            PhotonNetwork.ConnectUsingSettings();
        }

    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Successfully connected to Photon Master Server.");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
        loadingText.text = "Joining Lobby...";
    }

    public void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameManager.Instance = FindObjectOfType<GameManager>();
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
        // Only disable title if not returning to the main menu
        if (PhotonNetwork.InRoom)
        {
            title.SetActive(false);
        }

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
        roomNameText.text = PhotonNetwork.CurrentRoom.Name + " (" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + ")";


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

        // Update room information UI
        roomNameText.text = PhotonNetwork.CurrentRoom.Name + " (" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + ")";

        // Pass the player's ActorNumber (int) instead of the whole Player object
        photonView.RPC("UpdateScoreUI", RpcTarget.All, newPlayer.ActorNumber, GameManager.Instance.BirdLives, GameManager.Instance.birdScore);

        allPlayerNames.Add(newPlayerLabel);
    }

    [PunRPC]
    void UpdateScoreUI(int playerID, int BirdLives, int birdScore)
    {
        // Update the score for the player based on the playerID (int) passed from the RPC
        GameManager.Instance.UpdateScoreText(playerID, BirdLives, birdScore);
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ListAllPlayers();
        roomNameText.text = PhotonNetwork.CurrentRoom.Name + " (" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + ")";

        // Destroy player character
        GameObject playerObj = GameManager.Instance.GetPlayerObject(otherPlayer.ActorNumber); // Pass ActorNumber (int) instead of Player object
        if (playerObj)
            PhotonNetwork.Destroy(playerObj);

        // Destroy any held pig
        GameObject heldPig = GameManager.Instance.GetPlayerPig(otherPlayer.ActorNumber); // Pass ActorNumber (int) instead of Player object
        if (heldPig)
            PhotonNetwork.Destroy(heldPig);

        // Reassign farmer if needed
        CheckFarmerStatus();
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
        title.SetActive(true);
    }

    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        loadingText.text = "Disconnected: " + cause;
        menuButtons.SetActive(true);
        title.SetActive(true);
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        CloseMenus();
        loadingText.text = "Leaving Room";
        loadingScreen.SetActive(true);
        title.SetActive(true);
    }
    public override void OnLeftRoom()
    {
        CloseMenus();
        menuButtons.SetActive(true);
        title.SetActive(true);
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
        title.SetActive(true);
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
        loadingScreen.SetActive(true);
        loadingText.text = "Joining Room";
        
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
            title.SetActive(true);
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
    void AssignRoles()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            GameManager.Instance.SetRole(PhotonNetwork.LocalPlayer, "Farmer");
        }
        else
        {
            GameManager.Instance.SetRole(PhotonNetwork.LocalPlayer, "Bird");
        }
    }
    void CheckFarmerStatus()
    {
        Player[] players = PhotonNetwork.PlayerList;

        if (!GameManager.Instance.IsFarmerPresent())
        {
            if (players.Length <= 2)
            {
                GameManager.Instance.EndGame();
            }
            else
            {
                Player newFarmer = players[Random.Range(0, players.Length)];
                GameManager.Instance.SetRole(newFarmer, "Farmer");
                photonView.RPC("NotifyNewFarmer", RpcTarget.All, newFarmer.ActorNumber);
            }
        }
    }
    Dictionary<int, bool> readyPlayers = new Dictionary<int, bool>();

    public void ToggleReady()
    {
        bool isReady = !readyPlayers.ContainsKey(PhotonNetwork.LocalPlayer.ActorNumber);
        photonView.RPC("SetPlayerReady", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, isReady);
    }

    [PunRPC]
    void SetPlayerReady(int playerID, bool ready)
    {
        readyPlayers[playerID] = ready;
        CheckAllReady();
    }

    void CheckAllReady()
    {
        if (readyPlayers.Count == PhotonNetwork.CurrentRoom.PlayerCount && !readyPlayers.ContainsValue(false))
        {
            StartGame();
        }
    }

    [PunRPC]
    void NotifyNewFarmer(int farmerID)
    {
        Player farmer = PhotonNetwork.CurrentRoom.GetPlayer(farmerID);
        if (farmer != null)
        {
            Debug.Log(farmer.NickName + " is now the farmer.");
        }
    }


    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            CloseMenus();
            PhotonNetwork.LoadLevel(levelToPlay);
            photonView.RPC("CloseMenusForAll", RpcTarget.All);
        }
        else
        {
            Debug.Log("Not enough players to start the game.");
        }
    }
    public static Player GetPlayerByID(int playerID)
    {
        // Assuming you have a list of all players, you can search for the player with the given ID
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber == playerID)
            {
                return player;
            }
        }

        return null;  // If no player is found with the given ID
    }


    [PunRPC]
    public void CloseMenusForAll()
    {
        CloseMenus();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
