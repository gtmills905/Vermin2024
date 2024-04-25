
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 5.0f;
    public float destroyDelay = 3.0f;

    public void FireProjectile(Vector3 spawnPosition, Vector3 direction)
    {
        // Instantiate the projectile at the spawn position
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // Set the velocity with the given direction and speed
            rb.velocity = direction * projectileSpeed;

            // Set the rotation of the instantiated projectile to face the direction
            projectile.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

            // Destroy the projectile after a delay
            Destroy(projectile, destroyDelay);
        }
        else
        {
            Debug.LogError("The prefab does not contain a Rigidbody component.");
        }
    }
}
