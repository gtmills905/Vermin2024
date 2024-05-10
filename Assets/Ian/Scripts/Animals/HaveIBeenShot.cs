using UnityEngine;

public class HaveIBeenShot : MonoBehaviour
{
    public GameManager Manager;
    public RespawnManager respawnManager;
    public GameObject impactVFXPrefab; // Assign the VFX prefab in the Inspector

    void Start()
    {
        Manager = FindObjectOfType<GameManager>();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {

            // Instantiate the impact VFX at the collision point
            GameObject impactVFX = Instantiate(impactVFXPrefab, transform.position, Quaternion.identity);

            // Destroy the VFX after 2 seconds
            Destroy(impactVFX, 2f);

            Manager.BirdLives -= 1;
            respawnManager.RespawnPlayer();
        }
    }
}
