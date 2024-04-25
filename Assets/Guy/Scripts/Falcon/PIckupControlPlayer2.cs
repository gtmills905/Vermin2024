using UnityEngine;

public class PickupControlPlayer2 : MonoBehaviour
{
    public Rigidbody CurrentObject;
    public Transform PickupTarget;
    public bool AnimalAttached = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            // Check if an animal is not already attached
            if (!AnimalAttached)
            {
                // Get the Rigidbody of the collided object's parent
                Rigidbody targetRigidbody = other.GetComponentInParent<Rigidbody>();
                // Make sure the object has a Rigidbody
                if (targetRigidbody != null)
                {
                    CurrentObject = targetRigidbody;
                }
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            if (!AnimalAttached)
            {
                // Set the object's position to the pickup point
                CurrentObject.MovePosition(PickupTarget.position);
                AnimalAttached = true;
            }
            // If an animal is already attached, Ian you may want to add some feedback or interaction logic here
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            AnimalAttached = false;
            CurrentObject = null;
            // Reset any attached object references
        }
    }
}
