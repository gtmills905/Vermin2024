using UnityEngine;
using Photon.Pun;
using System.Collections;

public class PigCleanup : MonoBehaviour
{
    private bool isPlayerNearby = false;
    private float cleanupDelay = 0.2f;
    private Coroutine cleanupCoroutine;

    private PhotonView pigPhotonView;

    private void Awake()
    {
        pigPhotonView = GetComponent<PhotonView>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;

            // Stop cleanup if a player enters
            if (cleanupCoroutine != null)
            {
                StopCoroutine(cleanupCoroutine);
                cleanupCoroutine = null;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;

            // Start cleanup delay if no player is nearby
            if (cleanupCoroutine == null)
            {
                cleanupCoroutine = StartCoroutine(CheckForPlayersAndCleanup());
            }
        }
    }

    private IEnumerator CheckForPlayersAndCleanup()
    {
        while (true)
        {
            yield return new WaitForSeconds(cleanupDelay);

            // If no player is nearby, destroy the pig
            if (!isPlayerNearby && PhotonNetwork.IsMasterClient)
            {

                PigManager.UnregisterPig(gameObject);
                PhotonNetwork.Destroy(gameObject);
                PhotonNetwork.RemoveRPCs(pigPhotonView);
                yield break; // End coroutine after destruction
            }
        }
    }
}
