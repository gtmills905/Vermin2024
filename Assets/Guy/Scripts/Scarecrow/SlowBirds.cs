using UnityEngine;
using Photon.Pun;

public class SlowBirds : MonoBehaviour
{
    // When a player enters the slow-down zone
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView photonView = other.GetComponentInParent<PhotonView>();

            if (photonView != null && photonView.IsMine)  // Ensure we're interacting with the local player's object
            {
                BirdMovement birdMovement = photonView.GetComponent<BirdMovement>();

                if (birdMovement != null)
                {
                    birdMovement.inZone = true;  // Mark bird as in the slow zone
                    birdMovement.AdjustSpeeds(); // Slow down the bird
                }
            }
        }
    }

    // When a player exits the slow-down zone
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView photonView = other.GetComponentInParent<PhotonView>();

            if (photonView != null && photonView.IsMine)  // Ensure we're interacting with the local player's object
            {
                BirdMovement birdMovement = photonView.GetComponent<BirdMovement>();

                if (birdMovement != null)
                {
                    birdMovement.inZone = false;  // Mark bird as out of the slow zone
                    birdMovement.ResetSpeeds();  // Reset bird's speed to normal
                }
            }
        }
    }
}
