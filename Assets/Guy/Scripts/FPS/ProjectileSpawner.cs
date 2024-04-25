using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private void Start()
    {
        // Store the initial position and rotation
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    // Call this method whenever you want to reset the GameObject to its initial state
    public void ResetToInitial()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        // Add any additional reset behavior here
    }
}
