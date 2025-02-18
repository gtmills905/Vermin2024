using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Collections;

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

                foreach (var player in FindObjectsOfType<PickupControl>())
                {
                        if (player.photonView.IsMine)
                        {
                            player.photonView.RPC("AnimalDeposited", RpcTarget.All);
                            break;
                        }


                    
                    
                }
            
            

            StartCoroutine(DestroyPigWithDelay(other.gameObject));
            pigSpawnerUpdated.PigDestroyed();
        }
    }

    private IEnumerator DestroyPigWithDelay(GameObject pig)
    {
        yield return new WaitForSeconds(0.1f);
        PhotonView pigPhotonView = pig.GetComponent<PhotonView>();
        if (pigPhotonView != null && pigPhotonView.IsMine)
        {
            PhotonNetwork.Destroy(pig);
        }
    }



    public void UniversalDepositObject()
    {
        gameManager.DepositObject(1);
    }
}
