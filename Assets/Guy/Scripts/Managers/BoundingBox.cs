using UnityEngine;

public class BoundingBox : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Prevent the player from moving further
            Rigidbody playerRigidbody = other.transform.root.GetComponent<Rigidbody>();
            playerRigidbody.velocity = Vector3.zero;
        }
    }
}
