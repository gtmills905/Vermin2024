using System.Runtime.Serialization;
using UnityEngine;

public class PickupControl : MonoBehaviour
{
    public Transform pickupTarget; // This should be the attachment point on the bird
    public bool animalAttached = false;

    public Rigidbody currentObject;


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            // Check if an animal is not already attached
            if (!animalAttached)
            {
                // Get the Rigidbody of the collided object's parent
                Rigidbody targetRigidbody = other.GetComponentInParent<Rigidbody>();

                currentObject = targetRigidbody;
                AttachToObject();
                animalAttached = true;
            }
            else if(currentObject == null)
            {
                animalAttached = false;
            }
        }
    }
    void Update()
    {
        // Check if the attached object is null
        if (animalAttached && currentObject == null)
        {
            animalAttached = false;
        }
    }

    void FixedUpdate()
    {
        // If an animal is attached and the currentObject is not null, update its position to match the pickup target
        if (animalAttached && currentObject != null)
        {
            currentObject.MovePosition(pickupTarget.position);
        }
    }

    void AttachToObject()
    {
        // Check if currentObject is not null
        if (currentObject != null)
        {
            // Set the object's position relative to the attachment point
            currentObject.transform.position = pickupTarget.position;

            // Attach the object to the bird without changing its position and rotation
            currentObject.transform.SetParent(pickupTarget, false);

        }
    }

}
