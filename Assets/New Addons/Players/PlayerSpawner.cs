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

}