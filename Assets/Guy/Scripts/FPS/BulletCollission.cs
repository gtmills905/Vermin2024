using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    public GameObject impactVFXPrefab; // Assign the VFX prefab in the Inspector
    public Transform respawnPoint; // Assign the respawn point in the Inspector
    public GameManager gameManager; // Reference to the GameManager script

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Assuming the object you want to hit has the "Target" tag
        {
            // Instantiate the impact VFX at the collision point
            GameObject impactVFX = Instantiate(impactVFXPrefab, transform.position, Quaternion.identity);

            // Destroy the VFX after 2 seconds
            Destroy(impactVFX, 2f);

            // Remove all scripts except for Rigidbody and MeshCollider
            Component[] components = other.GetComponents<Component>();
            foreach (var component in components)
            {
                if (!(component is Rigidbody) && !(component is MeshCollider) && component != this)
                {
                    if (component.GetType() != typeof(BulletCollision))
                    {
                        Destroy(component);
                    }
                }
            }


            // Respawn the player target after 5 seconds
            Invoke("RespawnTarget", 5f);

            // Add a point to the farmer's score
            gameManager.FarmerScore += 1; // Assuming you have a public property for farmer score in GameManager
        }
    }

    private void RespawnTarget()
    {
        // Reset the position of the target to the respawn point
        transform.position = respawnPoint.position;

        // Reactivate the Rigidbody
        GetComponent<Rigidbody>().isKinematic = false;

        // Reactivate the MeshCollider
        GetComponent<MeshCollider>().enabled = true;
    }
}
