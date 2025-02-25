using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI; // Ensure you have this at the top
using Photon.Realtime;
using Microsoft.Win32.SafeHandles;
using System.Security.Permissions;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine.SceneManagement;
using System.Linq;
using ExitGames.Client.Photon;



public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;


    public GameObject readyButtonPrefab;  // Drag your ready button prefab here in the inspector
    public Transform parentTransform;     // The GameObject or Canvas where you want to parent the button


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

    public GameObject readyButton;
    public TMP_Text readyButtonText;
    // Replace the old dictionary with this one
    private Dictionary<int, ReadyPlayerData> readyPlayers = new Dictionary<int, ReadyPlayerData>();


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
        PhotonNetwork.ConnectUsingSettings();
        CloseMenus();
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Reconnecting...");
            PhotonNetwork.ConnectUsingSettings();
        }
        PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout = 10000; // 10 seconds
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 15;


        loadingScreen.SetActive(true);
        loadingText.text = "Connecting to Network...";



    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Successfully connected to Photon Master Server.");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
        loadingText.text = "Joining Lobby...";
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
        readyButton.SetActive(true);
        

        ListAllPlayers();

        if (PhotonNetwork.IsMasterClient && readyPlayers.Count == PhotonNetwork.CurrentRoom.PlayerCount && !readyPlayers.ContainsValue(false))
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
        // Destroy all old player name labels from the list
        foreach (TMP_Text player in allPlayerNames)
        {
            Destroy(player.gameObject);
        }
        allPlayerNames.Clear();

        // Loop through the current list of players and create a new label for each
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            TMP_Text newPlayerLabel = Instantiate(playerNameLabel, playerNameLabel.transform.parent);
            newPlayerLabel.text = players[i].NickName;

            newPlayerLabel.gameObject.SetActive(true);
            allPlayerNames.Add(newPlayerLabel);
        }
    }


    public override void OnLeftRoom()
    {
        CloseMenus();
        menuButtons.SetActive(true);
        title.SetActive(true);
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // Instantiate the ready button with the player's name
        GameObject newReadyButton = Instantiate(readyButtonPrefab, parentTransform);
        TextMeshProUGUI buttonText = newReadyButton.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = newPlayer.NickName;

        // Store the button and ready state in the dictionary
        readyPlayers[newPlayer.ActorNumber] = new ReadyPlayerData(newReadyButton);

        // Update room information UI
        roomNameText.text = PhotonNetwork.CurrentRoom.Name + " (" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + ")";

        // Check if the start button should be visible
        CheckAllReady();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // Remove the ready button when a player leaves
        if (readyPlayers.ContainsKey(otherPlayer.ActorNumber))
        {
            ReadyPlayerData playerData = readyPlayers[otherPlayer.ActorNumber];
            Destroy(playerData.readyButton); // Destroy the button associated with the player
            readyPlayers.Remove(otherPlayer.ActorNumber); // Remove from dictionary
        }

        // Refresh the list of players in the room UI
        ListAllPlayers();

        // Update room info
        roomNameText.text = PhotonNetwork.CurrentRoom.Name + " (" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + ")";

        // Handle ready button state for all players
        CheckAllReady();
    }



    private void RemovePlayerLabel(Player player)
    {
        // Find and remove the player's name label from the UI
        TMP_Text playerLabel = allPlayerNames.FirstOrDefault(label => label.text == player.NickName);
        if (playerLabel != null)
        {
            allPlayerNames.Remove(playerLabel);
            Destroy(playerLabel.gameObject); // Clean up the UI label
        }
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
        if (PhotonNetwork.IsMasterClient && readyPlayers.Count == PhotonNetwork.CurrentRoom.PlayerCount && !readyPlayers.ContainsValue(false))
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



    public void ToggleReady()
    {
        if (photonView.IsMine) // Only the local player can toggle their own readiness
        {
            // Create or get the existing ReadyPlayerData object
            ReadyPlayerData playerData = readyPlayers.ContainsKey(PhotonNetwork.LocalPlayer.ActorNumber)
                ? readyPlayers[PhotonNetwork.LocalPlayer.ActorNumber]
                : new ReadyPlayerData();

            // Toggle the readiness status
            playerData.isReady = !playerData.isReady;

            // Send the readiness update to all clients
            photonView.RPC("SetPlayerReady", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber, playerData);
        }
    }




    [PunRPC]
    void SetPlayerReady(int playerID, ReadyPlayerData playerData)
    {
        Debug.Log($"SetPlayerReady() called for Player {playerID}, Ready: {playerData.isReady}");

        // Update readiness dictionary
        if (!readyPlayers.ContainsKey(playerID))
        {
            readyPlayers.Add(playerID, playerData);
        }
        else
        {
            readyPlayers[playerID] = playerData;
        }

        // Update room properties (optional)
        Hashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
        roomProperties[$"ready_{playerID}"] = playerData.isReady;
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);

        // Update UI
        UpdateReadyButton(playerID, playerData.isReady);

        // Check if all players are ready
        CheckAllReady();
    }







    void UpdateReadyButton(int playerID, bool ready)
    {
        if (playerID == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            Button button = readyButton.GetComponent<Button>(); // Get the Button component
            button.interactable = true; // Local player can interact with their button

            // Ensure the correct TMP reference is used
            TextMeshProUGUI tmpText = readyButton.GetComponentInChildren<TextMeshProUGUI>();
            if (tmpText != null)
            {
                tmpText.text = ready ? "Ready" : "Not Ready";
            }
        }
        else
        {
            GameObject buttonObject = GameObject.Find($"ready status{playerID}");
            if (buttonObject != null)
            {
                Button btn = buttonObject.GetComponent<Button>();
                TextMeshProUGUI btnText = buttonObject.GetComponentInChildren<TextMeshProUGUI>();

                if (btn != null && btnText != null)
                {
                    btnText.text = ready ? "Ready" : "Not Ready";
                    btn.interactable = false; // Disable interaction for non-local players
                }
            }
        }
    }





    void CheckAllReady()
    {
        Debug.Log($"Checking ready status: {readyPlayers.Count}/{PhotonNetwork.CurrentRoom.PlayerCount}");

        // Ensure dictionary is updated for currently connected players
        readyPlayers = readyPlayers
            .Where(kvp => PhotonNetwork.CurrentRoom.Players.ContainsKey(kvp.Key))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        foreach (var player in readyPlayers)
        {
            Debug.Log($"Player {player.Key} Ready: {player.Value}");
        }
    }



    void InstantiateReadyButton()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            // Load the prefab from the Resources folder
            GameObject readyButtonPrefab = Resources.Load<GameObject>("ready status");

            // Debugging to check if prefab is loaded
            if (readyButtonPrefab != null)
            {
                Debug.Log("ReadyButtonPrefab loaded successfully.");

                // Now, instantiate the new object
                GameObject newReadyButton = Instantiate(readyButtonPrefab, parentTransform);

                PhotonView newPhotonView = newReadyButton.GetComponent<PhotonView>();

                // Allocate a unique ViewID for the new PhotonView
                PhotonNetwork.AllocateViewID(newPhotonView);

                // Set the button as a child of the specific parent
                newReadyButton.transform.SetParent(parentTransform, false); // Set to false to maintain world space positioning

                // Optionally, set the button position relative to the parent (UI only)
                newReadyButton.transform.localPosition = Vector3.zero;  // Adjust position as needed

                // Initialize the button UI (you may want to configure things like text, interaction, etc.)
                UpdateReadyButton(PhotonNetwork.LocalPlayer.ActorNumber, false); // Example of setting it to "Not Ready" initially

                // Get the Button component from the newly instantiated button
                Button button = newReadyButton.GetComponent<Button>();

                if (button != null)
                {
                    // Assign the ToggleReady method to the button's OnClick event
                    button.onClick.AddListener(ToggleReady);
                }
                else
                {
                    Debug.LogError("No Button component found on Ready Button prefab.");
                }
            }
            else
            {
                // Log if prefab is not found
                Debug.LogError("ReadyButtonPrefab not found in Resources/Prefabs.");
            }
        }
        else
        {
            Debug.LogError("PhotonNetwork is not connected or not in a room.");
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
public class ReadyPlayerData
{
    public GameObject readyButton;
    public bool isReady;

    // Constructor allows specifying the initial readiness state
    public ReadyPlayerData(GameObject button, bool ready = false)
    {
        readyButton = button;
        isReady = ready;  // Set initial readiness state
    }

}
