using UnityEngine;
using UnityEngine.SceneManagement;

public class DropOffSystem : MonoBehaviour
{
    public string targetTag = "Food";
    public AudioSource audioSource;
    public GameManager gameManager;
    public PigSpawnerUpdated pigSpawnerUpdated;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            audioSource.Play();
            UniversalDepositObject();
            Debug.Log("Item deposited. Total count: " + gameManager.birdScore);
            Destroy(other.gameObject);
            pigSpawnerUpdated.PigDestroyed();
        }
    }

    public void UniversalDepositObject()
    {
        gameManager.DepositObject(1);
    }
}
