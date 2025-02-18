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
        }
    }

    private void SpawnRandomPlayer()
    {
        int farmerActorNumber = GetFarmerActorNumber();

        if (farmerActorNumber == 0)
        {
            // No farmer yet — assign the first player to be the farmer
            if (PhotonNetwork.IsMasterClient)
            {
                SetFarmerActorNumber(PhotonNetwork.LocalPlayer.ActorNumber);
                RaiseFarmerEvent(PhotonNetwork.LocalPlayer.ActorNumber);
                SpawnFarmer();
            }
            else
            {
                SpawnPlayer(); // Regular players just spawn as non-farmers
            }
        }
        else
        {
            // A farmer is already assigned, so just spawn as a regular player
            if (PhotonNetwork.LocalPlayer.ActorNumber == farmerActorNumber)
            {
                SpawnFarmer();
            }
            else
            {
                SpawnPlayer();
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        if (otherPlayer.ActorNumber == GetFarmerActorNumber())
        {
            Debug.Log("Farmer has disconnected. Assigning a new farmer...");
            SetFarmerActorNumber(0);

            // Assign a new farmer — let the MasterClient decide
            if (PhotonNetwork.IsMasterClient && PhotonNetwork.PlayerList.Length > 0)
            {
                int newFarmerActorNumber = PhotonNetwork.PlayerList[0].ActorNumber;
                SetFarmerActorNumber(newFarmerActorNumber);
                RaiseFarmerEvent(newFarmerActorNumber);
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        // Sync farmer status with the joining player
        int farmerActorNumber = GetFarmerActorNumber();

        if (farmerActorNumber == 0 && PhotonNetwork.IsMasterClient)
        {
            // If no farmer is assigned, assign the new player as the farmer
            SetFarmerActorNumber(newPlayer.ActorNumber);
            RaiseFarmerEvent(newPlayer.ActorNumber);
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
