using UnityEngine;

public class GunShake : MonoBehaviour
{
    public Transform gunTransform; // The transform of the gun
    private float shakeAmount = .2f; // Adjust the amount of shake
    private float shakeSpeed = 0.2f; // Adjust the speed of shake

    private Vector3 originalGunRotation;
    private Vector3 originalCameraRotation;

    void Start()
    {
        if (gunTransform == null)
            Debug.LogError("Gun transform not assigned!");

        originalGunRotation = gunTransform.localEulerAngles;
        originalCameraRotation = transform.localEulerAngles;
    }

    void Update()
    {
        // Calculate the shake amounts
        float gunShakeX = Mathf.PerlinNoise(Time.time * shakeSpeed, 0) * 2 - 1;
        float gunShakeY = Mathf.PerlinNoise(Time.time * shakeSpeed, 1) * 2 - 1;
        float gunShakeZ = Mathf.PerlinNoise(Time.time * shakeSpeed, 2) * 2 - 1;

        // Apply shake to the gun
        gunTransform.localEulerAngles = originalGunRotation + new Vector3(gunShakeX, gunShakeY, gunShakeZ) * shakeAmount;

        // Apply shake to the camera
        float cameraShakeX = Mathf.PerlinNoise(Time.time * shakeSpeed, 3) * 2 - 1;
        float cameraShakeY = Mathf.PerlinNoise(Time.time * shakeSpeed, 4) * 2 - 1;
        float cameraShakeZ = Mathf.PerlinNoise(Time.time * shakeSpeed, 5) * 2 - 1;

        transform.localEulerAngles = originalCameraRotation + new Vector3(cameraShakeX, cameraShakeY, cameraShakeZ) * shakeAmount;
    }
}
