using UnityEngine;

public class PickupControlPlayer2 : MonoBehaviour
{
    public Transform pickupTarget; // This should be the attachment point on the bird
    public bool animalAttached = false;

    private Rigidbody currentObject;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            // Check if an animal is not already attached
            if (!animalAttached)
            {
                // Get the Rigidbody of the collided object's parent
                Rigidbody targetRigidbody = other.GetComponentInParent<Rigidbody>();

                if (targetRigidbody != null)
                {
                    currentObject = targetRigidbody;
                    AttachToObject();
                    animalAttached = true;
                }
            }
        }
    }

    void FixedUpdate()
    {
        // If an animal is attached, update its position to match the pickup target
        if (animalAttached && currentObject != null)
        {
            currentObject.MovePosition(pickupTarget.position);
        }
    }

    void AttachToObject()
    {
        // Set the object's position relative to the attachment point
        currentObject.transform.position = pickupTarget.position;

        // Attach the object to the bird without changing its position and rotation
        currentObject.transform.SetParent(pickupTarget, false);
    }
}
