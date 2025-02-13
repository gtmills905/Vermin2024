using UnityEngine;
using UnityEngine.SceneManagement;

public class DropOffSystem2 : MonoBehaviour
{
    public int totalCount = 0;
    public int winCount = 11;
    public string targetTag = "Food";
    public AudioSource audioSource;

    public Pingspawner pingspawner;// added by ian

    void Start()//ian
    {
        GameObject spawnObject = GameObject.Find("spawn"); //ian
        pingspawner = spawnObject.GetComponent<Pingspawner>(); // ian
        

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            totalCount++;
            pingspawner.StartCoroutine(pingspawner.NewPig());//ian
            audioSource.Play();
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
}
