using System.Collections;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public GameObject player;
    public GameObject spawnPoint;
    private bool isRespawning = false;

    public void RespawnPlayer()
    {
        if (!isRespawning)
        {
            isRespawning = true;
            player.transform.position = spawnPoint.transform.position;
            player.SetActive(false);
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
