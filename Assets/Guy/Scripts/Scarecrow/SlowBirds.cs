using UnityEngine;
using Photon.Pun;

public class SlowBirds : MonoBehaviourPunCallbacks
{
    private BirdMovement birdMovement;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView photonView = other.GetComponentInParent<PhotonView>();

            if (photonView != null && photonView.IsMine)
            {
                birdMovement = other.GetComponent<BirdMovement>();
                if (birdMovement != null)
                {
                    birdMovement.inZone = true;
                    birdMovement.AdjustSpeeds();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView photonView = other.GetComponentInParent<PhotonView>();

            if (photonView != null && photonView.IsMine)
            {
                birdMovement = other.GetComponent<BirdMovement>();
                if (birdMovement != null)
                {
                    birdMovement.inZone = false;
                    birdMovement.AdjustSpeeds(); // Will check if carrying an animal before resetting speed
                }
            }
        }
    }
}