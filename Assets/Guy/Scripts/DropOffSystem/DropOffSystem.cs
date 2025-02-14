using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class DropOffSystem : MonoBehaviourPunCallbacks
{
    public string targetTag = "Food";
    public AudioSource audioSource;
    public GameManager gameManager;
    public PigSpawnerUpdated pigSpawnerUpdated;
    private PickupControl pickupControl;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            audioSource.Play();
            UniversalDepositObject();

            // Find the player who owns this object
            PhotonView pigPhotonView = other.GetComponent<PhotonView>();
            if (pigPhotonView != null && pigPhotonView.Owner != null)
            {
                // Find the PickupControl of the owner
                foreach (var player in FindObjectsOfType<PickupControl>())
                {
                    if (player.photonView.Owner == pigPhotonView.Owner)
                    {
                        player.photonView.RPC("AnimalDeposited", player.photonView.Owner);
                        break;
                    }
                }
            }

            Destroy(other.gameObject);
            pigSpawnerUpdated.PigDestroyed();
        }
    }

    public void UniversalDepositObject()
    {
        gameManager.DepositObject(1);
    }
}
