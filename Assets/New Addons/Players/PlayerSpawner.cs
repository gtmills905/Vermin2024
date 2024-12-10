using UnityEngine;
using Photon.Pun;
using System.Collections;
using System.Security.Cryptography;
using System.Threading;

public class PlayerSpawner : MonoBehaviour
{




    public static PlayerSpawner Instance;

    private void Awake()
    {
        Instance = this;
    }
    private bool isRespawning = false;
    public PickupControl pickupControl;


    public GameObject playerPrefab;
    private GameObject player;
    public GameObject deathEffect;

    public float respawnTime = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
        }
        
    }
    public void SpawnPlayer()
    {
        Transform spawnPoint = RespawnManager.instance.GetSpawnPoint();

        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RespawnPlayer()
    {
        if (!isRespawning)
        {
            isRespawning = true;
            
            player.SetActive(false);

            if (!player.activeSelf)
            {
                if (pickupControl != null && pickupControl.currentObject != null)
                {
                    Destroy(pickupControl.currentObject.gameObject);
                }
            }

            StartCoroutine(ReactivatePlayer());
        }
    }
    IEnumerator ReactivatePlayer()
    {
        yield return new WaitForSeconds(5f);
        player.SetActive(true);
        isRespawning = false;
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

        SpawnPlayer();
    }
}
