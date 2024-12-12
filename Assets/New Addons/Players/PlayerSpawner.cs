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
        Instance = this;
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
        Transform spawnPoint = RespawnManager.instance.GetSpawnPoint();

        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
        // After spawning, check the tag of the instantiated object
        CheckPlayerTag(player);
    }

    public void SpawnFarmer()
    {
        Transform farmerSpawnPoint = RespawnManager.instance.GetFarmerSpawnPoint();

        farmer = PhotonNetwork.Instantiate(farmerPrefab.name, farmerSpawnPoint.position, farmerSpawnPoint.rotation);

        // After spawning, check the tag of the instantiated object
        CheckPlayerTag(farmer);
    }


    public void Die(string damager)
    {
        

        UiController.instance.deathText.text = "You were killed by " + damager;

        if (player!= null)
        {
            StartCoroutine(DieCo());
        }

    }
    public IEnumerator DieCo()
    {
        PhotonNetwork.Instantiate(deathEffect.name, player.transform.position, Quaternion.identity);

        PhotonNetwork.Destroy(player);
        UiController.instance.deathScreen.SetActive(true);

        yield return new WaitForSeconds(respawnTime);

        UiController.instance.deathScreen.SetActive(false);
        // Check if this is the local player
        if (photonView.IsMine)
        {
            // Find PickupControl script on this object or its parent
            PickupControl pickupControl = GetComponent<PickupControl>()
                                         ?? GetComponentInParent<PickupControl>();

            // If PickupControl script and a held object exist, destroy the held object
            if (pickupControl != null && pickupControl.currentObject != null)
            {
                PhotonNetwork.Destroy(pickupControl.currentObject.gameObject);
            }


            SpawnPlayer();
        }
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
