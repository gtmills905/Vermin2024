using UnityEngine;

public class GunController : MonoBehaviour
{
    public GameObject projectilePrefab;
    public ProjectileController projectileController;
    public GameObject particleEffectPrefab;
    public AudioSource gunShotSoundAudioSource;
    public AudioClip gunShotSoundClip;

    public float reloadTime = 5.0f;
    public Transform spawnPosition;

    // Adjust these variables for the shotgun spread
    public int numberOfProjectiles = 10;
    public float coneSpreadAngle = 20f;

    private bool canShoot = true;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            Shoot();
            canShoot = false;
            Invoke("Reload", reloadTime);
        }
    }

    private void Shoot()
    {
        // Instantiate particle effect at the specified position
        GameObject particleEffect = Instantiate(particleEffectPrefab, spawnPosition.position, Quaternion.identity);

        // Destroy the particle effect after 2 seconds
        Destroy(particleEffect, 2f);

        for (int i = 0; i < numberOfProjectiles; i++)
        {
            // Calculate normalized direction based on spawn position and cone spread
            float angle = i * (coneSpreadAngle / numberOfProjectiles) - (coneSpreadAngle / 2f);
            Quaternion rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 direction = rotation * spawnPosition.forward;

            // Fire projectile with the updated spawn position and direction
            projectileController.FireProjectile(spawnPosition.position, direction);
        }

        if (gunShotSoundAudioSource != null && gunShotSoundClip != null)
        {
            gunShotSoundAudioSource.PlayOneShot(gunShotSoundClip);
        }
    }

    private void Reload()
    {
        canShoot = true;
    }
}
