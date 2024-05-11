using System.Collections;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public GameObject player;
    public GameObject spawnPoint;
    private bool isRespawning = false;
    public PickupControl pickupControl;

    public void RespawnPlayer()
    {
        if (!isRespawning)
        {
            isRespawning = true;
            player.transform.position = spawnPoint.transform.position;
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
        yield return new WaitForSeconds(20f);
        player.SetActive(true);
        isRespawning = false;
    }
}
