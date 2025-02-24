using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;


public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    private const string FarmerActorKey = "FarmerActor"; // Custom property key to store the farmer's ActorNumber
    public static PlayerSpawner Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject playerPrefab;
    private GameObject player;
    public GameObject deathEffect;

    public GameObject farmerPrefab;
    private GameObject farmer;

    public float respawnTime = 5f;

    void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.LocalPlayer.IsLocal)
        {
            // Randomize spawn type: either "Farmer" or something else
            SpawnRandomPlayer();
            PhotonNetwork.AutomaticallySyncScene = true;

        }
    }

    private void SpawnRandomPlayer()
    {
        int farmerActorNumber = GetFarmerActorNumber();

        if (farmerActorNumber == 0)
        {

                SetFarmerActorNumber(PhotonNetwork.LocalPlayer.ActorNumber);
                RaiseFarmerEvent(PhotonNetwork.LocalPlayer.ActorNumber);
                SpawnFarmer();

        }
        else
        {

                SpawnPlayer();
            
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        if (otherPlayer.ActorNumber == GetFarmerActorNumber())
        {
            Debug.Log("Farmer has disconnected. Assigning a new farmer...");
            SetFarmerActorNumber(0); // Reset farmer role

            // Assign a new farmer — MasterClient takes over
            if (PhotonNetwork.IsMasterClient && PhotonNetwork.PlayerList.Length > 0)
            {
                int newFarmerActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                SetFarmerActorNumber(newFarmerActorNumber);
                RaiseFarmerEvent(newFarmerActorNumber);

                // Transfer ownership of pigs and scarecrow to the new Farmer
                TransferFarmObjectsOwnership();
            }
        }
    }
    private void TransferFarmObjectsOwnership()
    {
        // Find all pig and scarecrow objects (or whatever the Farmer controls)
        GameObject[] pigs = GameObject.FindGameObjectsWithTag("Food");
        GameObject scarecrow = GameObject.FindWithTag("Scarecrow");

        // Transfer ownership to the new Farmer (MasterClient now)
        foreach (GameObject pig in pigs)
        {
            PhotonView pigView = pig.GetComponent<PhotonView>();
            if (pigView != null && !pigView.IsMine)
            {
                pigView.TransferOwnership(PhotonNetwork.LocalPlayer);
            }
        }

        if (scarecrow != null)
        {
            PhotonView scarecrowView = scarecrow.GetComponent<PhotonView>();
            if (scarecrowView != null && !scarecrowView.IsMine)
            {
                scarecrowView.TransferOwnership(PhotonNetwork.LocalPlayer);
            }
        }

        Debug.Log("Farm objects ownership transferred to the new Farmer!");
    }



    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        // Sync farmer status with the joining player
        int farmerActorNumber = GetFarmerActorNumber();

        if (newPlayer.ActorNumber == farmerActorNumber)
        {
            photonView.RPC("RPC_SpawnFarmerForNewPlayer", newPlayer);
        }
        else
        {
            photonView.RPC("RPC_SpawnPlayerForNewPlayer", newPlayer);
        }
    }

    [PunRPC]
    void RPC_SpawnFarmerForNewPlayer()
    {
        SpawnFarmer();
    }

    [PunRPC]
    void RPC_SpawnPlayerForNewPlayer()
    {
        SpawnPlayer();
    }


    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        // Spawn yourself based on role
        int farmerActorNumber = GetFarmerActorNumber();

        if (PhotonNetwork.LocalPlayer.ActorNumber == farmerActorNumber)
        {
            SpawnFarmer();
        }
        else
        {
            SpawnPlayer();
        }

        // Sync existing players with the new joiner
        foreach (Player player in PhotonNetwork.PlayerListOthers)
        {
            if (player.ActorNumber == farmerActorNumber)
            {
                photonView.RPC("RPC_SpawnFarmerForNewPlayer", PhotonNetwork.LocalPlayer);
            }
            else
            {
                photonView.RPC("RPC_SpawnPlayerForNewPlayer", PhotonNetwork.LocalPlayer);
            }
        }
    }


    private void RaiseFarmerEvent(int actorNumber)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All, // Send event to all players
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = true // Make the event reliable
        };

        PhotonNetwork.RaiseEvent(0, actorNumber, raiseEventOptions, sendOptions);
    }

    public void SpawnPlayer()
    {
        if (!PhotonNetwork.IsConnected) return;

        Transform spawnPoint = RespawnManager.instance.GetSpawnPoint();
        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
        CheckPlayerTag(player);
    }

    public void SpawnFarmer()
    {
        if (!PhotonNetwork.IsConnected) return;

        Transform farmerSpawnPoint = RespawnManager.instance.GetFarmerSpawnPoint();
        farmer = PhotonNetwork.Instantiate(farmerPrefab.name, farmerSpawnPoint.position, farmerSpawnPoint.rotation);
        CheckPlayerTag(farmer);

        PhotonView farmerView = farmer.GetComponent<PhotonView>();
        if (farmerView != null && farmerView.IsMine) // Check if this is the local player's farmer
        {
            DisableMainCamera();
        }
    }

    private void DisableMainCamera()
    {
        GameObject mainCamera = GameObject.Find("Main Camera");
        if (mainCamera != null)
        {
            mainCamera.SetActive(false);
        }
    }

    public void RespawnPlayerAfterDelay()
    {
        StartCoroutine(RespawnPlayerCoroutine());
    }

    private IEnumerator RespawnPlayerCoroutine()
    {
        yield return new WaitForSeconds(respawnTime);

        if (UiController.instance != null)
        {
            UiController.instance.deathScreen.SetActive(false);
        }

        SpawnPlayer();
    }

    private void CheckPlayerTag(GameObject targetObject)
    {
        if (targetObject.CompareTag("Player"))
        {
            UiController.instance.birdUI.SetActive(true);
            UiController.instance.farmerUI.SetActive(false);
        }
        else if (targetObject.CompareTag("Farmer"))
        {
            UiController.instance.farmerUI.SetActive(true);
            UiController.instance.birdUI.SetActive(false);
        }
    }

    private int GetFarmerActorNumber()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(FarmerActorKey))
        {
            return (int)PhotonNetwork.CurrentRoom.CustomProperties[FarmerActorKey];
        }
        return 0;
    }

    private void SetFarmerActorNumber(int actorNumber)
    {
        ExitGames.Client.Photon.Hashtable customProps = new ExitGames.Client.Photon.Hashtable();
        customProps[FarmerActorKey] = actorNumber;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProps);
    }

    
}
