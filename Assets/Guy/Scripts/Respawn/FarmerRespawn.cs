using System.Collections;
using UnityEngine;

public class FarmerRespawn : MonoBehaviour
{
    public SC_FPSController sC_FPSController;
    public Transform spawnPoint;  // Reference to the spawn point

    void Start()
    {
        // Find the spawn point in the scene
        spawnPoint = GameObject.Find("Farmer Respawn").transform;
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
        sC_FPSController.enabled = false;  // Disable player controls
        yield return new WaitForSeconds(.5f);
        // Teleport the player to the spawn point
        farmer.transform.position = spawnPoint.position;
        yield return new WaitForSeconds(.5f);
        sC_FPSController.enabled = true;  // Enable player controls
    }
}

