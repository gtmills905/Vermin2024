using UnityEngine;
using UnityEngine.SceneManagement;

public class DropOffSystem : MonoBehaviour
{
    public int totalCount = 0;
    public int winCount = 11;
    public string targetTag = "Food";
    public AudioSource audioSource;
    public GameManager gamemanager;
    public PickupControlPlayer1 pickupControl1;
    public PickupControlPlayer2 pickupControl2;
    public PickupControlPlayer3 pickupControl3;

    public Pigspawner pigspawner;// added by ian

    void Start()//ian
    {
        GameObject spawnObject = GameObject.Find("Target Spawn"); //ian
        pigspawner = spawnObject.GetComponent<Pigspawner>(); // ian


    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            if (pickupControl1.animalAttached1 == true)
            {
                pickupControl1.animalAttached1 = false;

                totalCount++;

                audioSource.Play();

                Deposit();

                Debug.Log("Item deposited. Total count: " + totalCount);

                Destroy(other.gameObject); print("die");

                pigspawner.StartCoroutine(pigspawner.NewPig());//ian
            }
            if (pickupControl2.animalAttached2 == true)
            {
                pickupControl2.animalAttached2 = false;

                totalCount++;

                audioSource.Play();

                Deposit();

                Debug.Log("Item deposited. Total count: " + totalCount);

                Destroy(other.gameObject); print("die");

                pigspawner.StartCoroutine(pigspawner.NewPig());//ian
            }
            if (pickupControl3.animalAttached3 == true)
            {
                pickupControl3.animalAttached3 = false;

                totalCount++;

                audioSource.Play();

                Deposit();

                Debug.Log("Item deposited. Total count: " + totalCount);

                Destroy(other.gameObject); print("die");

                pigspawner.StartCoroutine(pigspawner.NewPig());//ian
            }
        }
        if (totalCount >= winCount)
        {
            SceneManager.LoadScene("WinSceneBirds");//for birds
        }
    }
    public void Deposit ()
    {
        
        
        if(pickupControl1.animalAttached1 == false)
        {
            gamemanager.DepositObject(1);
        }
        if (pickupControl2.animalAttached2 == false)
        {
            gamemanager.DepositObject(1);
        }
        if (pickupControl3.animalAttached3 == false)
        {
            gamemanager.DepositObject(1);
        }

    }
}
