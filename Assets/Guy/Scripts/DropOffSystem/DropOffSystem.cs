using UnityEngine;
using UnityEngine.SceneManagement;

public class DropOffSystem : MonoBehaviour
{
    public int totalCount = 0;
    public int winCount = 15;
    public string targetTag = "Food";
    public AudioSource audioSource;
    public GameManager gameManager;
    public PickupControlPlayer1 pickupControlPlayer1;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            totalCount++;
            audioSource.Play();
            DepositObject();
            Debug.Log("Item deposited. Total count: " + totalCount);
            Destroy(other.gameObject);
            if (totalCount >= winCount)
            {
                
                {
                    SceneManager.LoadScene("WinScene");
                }
               
            }
        }
    }
    public void DepositObject()
    {
        gameManager.DepositObject(1);
        pickupControlPlayer1.animalAttached = false;
    }
}
