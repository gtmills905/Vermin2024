using UnityEngine;

public class TargetEffects : MonoBehaviour
{
    public AudioSource audioSource;
    public GameObject particleEffect; // Assign the particle effect prefab in the Unity Editor

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Food"))
        {

            // Instantiate the particle effect
            Instantiate(particleEffect, other.gameObject.transform.position, Quaternion.identity);


            audioSource.Play();
        }
    }
}
