using UnityEngine;
using Photon.Pun;
using System.Collections;
using System.Security.Cryptography;
using System.Threading;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{

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

    public PickupControl pickupControl;


    public GameObject playerPrefab;
    private GameObject player;
    public GameObject deathEffect;

    public GameObject farmerPrefab;
    private GameObject farmer;

    public float respawnTime = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
            {
                SpawnPlayer();
            }
            if (PhotonNetwork.LocalPlayer.ActorNumber >= 3)
            {
                SpawnPlayer();
            }
            else if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
            {
                SpawnFarmer();
            }


        }
        

    }
    public void SpawnPlayer()
    {
        if (!PhotonNetwork.IsConnected) return; // Only proceed if connected to Photon

        Transform spawnPoint = RespawnManager.instance.GetSpawnPoint();
        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
        CheckPlayerTag(player);
    }

    public void SpawnFarmer()
    {
        if (!PhotonNetwork.IsConnected) return; // Only proceed if connected to Photon

        Transform farmerSpawnPoint = RespawnManager.instance.GetFarmerSpawnPoint();
        farmer = PhotonNetwork.Instantiate(farmerPrefab.name, farmerSpawnPoint.position, farmerSpawnPoint.rotation);
        CheckPlayerTag(farmer);
    }

    public void RespawnPlayerAfterDelay()
    {
        StartCoroutine(RespawnPlayerCoroutine());
    }

    private IEnumerator RespawnPlayerCoroutine()
    {
        yield return new WaitForSeconds(respawnTime); // Wait before respawning

        // Hide death screen
        if (UiController.instance != null)
        {
            UiController.instance.deathScreen.SetActive(false);
        }

        // Spawn new player
        SpawnPlayer();
    }


    // Method to check the tag of the player or farmer object
    private void CheckPlayerTag(GameObject targetObject)
    {
        if (targetObject.CompareTag("Player"))
        {
            // Handle UI for player
            UiController.instance.birdUI.SetActive(true);
            UiController.instance.farmerUI.SetActive(false);
        }
        else if (targetObject.CompareTag("Farmer"))
        {
            // Handle UI for farmer
            UiController.instance.farmerUI.SetActive(true);
            UiController.instance.birdUI.SetActive(false);
        }
    }
}
