using UnityEngine;

public class RagdollActivation : MonoBehaviour
{
    public GameObject ragdoll; // Assign your ragdoll GameObject here

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Adjust the tag as per your requirement
        {
            EnableRagdoll();
        }
    }

    void EnableRagdoll()
    {
        ragdoll.SetActive(true);

        Rigidbody[] rigidbodies = ragdoll.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = false; // Enable physics simulation
        }

        Collider[] colliders = ragdoll.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.isTrigger = false; // Disable trigger to enable physics collision
        }
    }
}
