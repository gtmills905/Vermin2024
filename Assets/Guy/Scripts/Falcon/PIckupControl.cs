using UnityEngine;
using Photon.Pun;

public class PickupControl : MonoBehaviourPunCallbacks
{
    public Transform pickupTarget;  // The attachment point on the bird
    public bool animalAttached = false;
    private GameObject currentObject; // Store the attached object

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food") && !animalAttached)
        {
            PhotonView targetView = other.GetComponentInParent<PhotonView>();

            if (targetView != null)
            {
                photonView.RPC("AttachToObject", RpcTarget.AllBuffered, targetView.ViewID);
            }
        }
    }

    [PunRPC]
    void AttachToObject(int objectId)
    {
        // Find the object by its PhotonView ID
        GameObject targetObject = PhotonView.Find(objectId)?.gameObject;

        if (targetObject == null)
        {
            Debug.LogError($"PickupControl: Could not find object with PhotonView ID {objectId}");
            return;
        }

        if (pickupTarget == null)
        {
            Debug.LogError("PickupControl: Missing pickup target.");
            return;
        }

        // Stop pig movement
        PigWandererUpdated pigWander = targetObject.GetComponent<PigWandererUpdated>();
        if (pigWander != null)
        {
            pigWander.enabled = false;
        }

        // Re-parent and move the object directly onto the pickup target
        targetObject.transform.SetParent(pickupTarget, worldPositionStays: false);
        targetObject.transform.localPosition = Vector3.zero;  // Ensure it snaps to the pickup point
        targetObject.transform.localRotation = Quaternion.identity; // Reset rotation

        // Sync the current object reference
        currentObject = targetObject;
        animalAttached = true;

        Debug.Log("PickupControl: Object attached to pickup target.");
    }

    public void RequestDetach()
    {
        if (currentObject != null)
        {
            photonView.RPC("DetachObject", RpcTarget.AllBuffered);
        }
    }
    [PunRPC]
    public void AnimalDeposited()
    {
        animalAttached = false;
    }

    [PunRPC]
    void DetachObject()
    {
        if (currentObject == null)
        {
            Debug.LogWarning("PickupControl: No object to detach.");
            return;
        }

        // Re-enable pig movement
        PigWandererUpdated pigWander = currentObject.GetComponent<PigWandererUpdated>();
        if (pigWander != null)
        {
            pigWander.enabled = true;
        }

        // Re-enable physics
        Rigidbody rb = currentObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.None;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Remove parent (detach)
        currentObject.transform.SetParent(null, worldPositionStays: true);
        currentObject = null;
        animalAttached = false;

        Debug.Log("PickupControl: Object detached.");
    }
}
