using UnityEngine;

public class StopFoodCollidersFromPassing : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Check if this object or the colliding object is tagged as "Food"
        if (gameObject.CompareTag("Food") || collision.gameObject.CompareTag("Food"))
        {
            // Calculate the separation vector to move this object out of collision
            Vector3 separationVector = CalculateSeparationVector(collision);

            // Move this object along the separation vector to resolve collision
            transform.position += separationVector;
        }
    }

    private Vector3 CalculateSeparationVector(Collision collision)
    {
        Vector3 separationVector = Vector3.zero;

        foreach (ContactPoint contact in collision.contacts)
        {
            // Calculate the separation direction from the contact point
            Vector3 separationDir = contact.normal * contact.separation;

            // Accumulate separation vectors from each contact point
            separationVector += separationDir;
        }

        return separationVector;
    }
}
