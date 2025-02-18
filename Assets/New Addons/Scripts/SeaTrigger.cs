
using UnityEngine;

public class SeaTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the pig (tagged "Food")
        if (other.CompareTag("Food"))
        {
            // Get the PickupControl component from the pig (which should be a child of the player or attached)
            PickupControl pickupControl = other.GetComponentInParent<PickupControl>();

            if (pickupControl != null)
            {
                // Detach the pig from the player
                pickupControl.animalAttached = false;

                // Call the DetachObject method to detach and destroy the pig
                pickupControl.RequestDetach();
            }

            // Destroy the pig (food) object
            Destroy(other.gameObject);

            // Log to confirm destruction and detachment
            Debug.Log("Pig (Food) destroyed and detached.");
        }
    }
}
