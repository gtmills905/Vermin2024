using System.Collections;
using UnityEngine;
using Photon.Pun;


public class FarmerRespawn : MonoBehaviourPunCallbacks
{
    public Transform spawnPoint;  // Reference to the spawn point

    void Start()
    {
        RespawnManager.instance = FindObjectOfType<RespawnManager>();  // Dynamically find the instance
        if (RespawnManager.instance == null)
        {
            Debug.LogError("RespawnManager instance not found in the scene.");
        }
        else
        {
            spawnPoint = RespawnManager.instance.GetFarmerSpawnPoint();
        }
    }

    // This function is called when the player enters the trigger collider
    void OnTriggerEnter(Collider other)
    {
        // Check if the entered object is tagged as "Farmer"
        if (other.CompareTag("Farmer"))
        {
            StartCoroutine(FarmerRespawnMethod(other));
        }
    }

    IEnumerator FarmerRespawnMethod(Collider farmer)
    {
        yield return new WaitForSeconds(0.5f);

        // Get the farmer's PhotonView
        PhotonView farmerPhotonView = farmer.GetComponent<PhotonView>();

        if (farmerPhotonView != null)
        {
            // Call RPC to respawn the farmer on all clients (so all instances are synchronized)
            photonView.RPC("RPC_RespawnFarmer", RpcTarget.All, farmerPhotonView.ViewID, spawnPoint.position);
        }
        else
        {
            Debug.LogError("No PhotonView found on the farmer.");
        }
    }

    // This is the actual RPC method that runs on all clients
    [PunRPC]
    void RPC_RespawnFarmer(int farmerViewID, Vector3 position)
    {
        PhotonView farmerPhotonView = PhotonView.Find(farmerViewID);

        if (farmerPhotonView != null)
        {
            // Move the farmer to the new spawn position
            farmerPhotonView.transform.position = position;
        }
        else
        {
            Debug.LogError($"PhotonView with ID {farmerViewID} not found!");
        }
    }
}
