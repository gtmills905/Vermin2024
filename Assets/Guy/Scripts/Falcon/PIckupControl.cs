using UnityEngine;
using Photon.Pun;
using System.Collections;

public class PickupControl : MonoBehaviourPunCallbacks
{
    public Transform pickupTarget;  // The attachment point on the bird
    public bool animalAttached = false;
    public GameObject currentObject; // Store the attached object

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food") && !animalAttached)  // Prevent picking up more than one pig
        {
            PhotonView targetView = other.GetComponentInParent<PhotonView>();

            if (targetView != null && !animalAttached)  // Double-check here for safety
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

        // Transfer ownership to the local player if necessary
        PhotonView targetPhotonView = targetObject.GetComponent<PhotonView>();
        if (targetPhotonView != null && !targetPhotonView.IsMine)
        {
            targetPhotonView.TransferOwnership(PhotonNetwork.LocalPlayer);  // Transfer ownership
            Debug.Log($"PickupControl: Ownership of {targetObject.name} transferred to {PhotonNetwork.LocalPlayer.NickName}.");
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
            photonView.RPC("DestroyAttachedPig", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void DestroyAttachedPig()
    {
        if (currentObject == null)
        {
            Debug.LogWarning("PickupControl: No object to destroy.");
            return;
        }

        PhotonView pigPhotonView = currentObject.GetComponent<PhotonView>();
        if (pigPhotonView != null)
        {
            // Ensure the local player has ownership before destroying
            if (!pigPhotonView.IsMine)
            {
                // Transfer ownership before destroying
                pigPhotonView.TransferOwnership(PhotonNetwork.LocalPlayer);
                Debug.Log($"PickupControl: Ownership of {currentObject.name} transferred to {PhotonNetwork.LocalPlayer.NickName}.");
            }

            // Destroy the object across the network
            PhotonNetwork.Destroy(currentObject);  // Destroys the object across all clients
            Debug.Log("PickupControl: Attached pig destroyed.");
        }
        else
        {
            Debug.LogError("PickupControl: No PhotonView found on the object.");
        }

        // Reset the bird's state
        currentObject = null;
        animalAttached = false;

        // After destroying or deactivating the object, now trigger the player's death if needed.
        StartCoroutine(DelayPlayerDeath());
    }

    // Coroutine to delay player death
    private IEnumerator DelayPlayerDeath()
    {
        // Wait for a frame to allow the destruction process to complete
        yield return null;

        // Trigger player death
        BirdMovement birdMovement = GetComponent<BirdMovement>();
        if (birdMovement != null)
        {
            birdMovement.Die("Player has died.");
        }

        Debug.Log("Player death triggered after object destruction.");
    }

    [PunRPC]
    public void AnimalDeposited()
    {
        animalAttached = false;
        currentObject = null;
        Debug.Log("PickupControl: Animal deposited. Ready to pick up another.");

        // Notify BirdMovement to reset speeds after depositing the animal
        BirdMovement birdMovement = GetComponent<BirdMovement>();
        if (birdMovement != null && !birdMovement.inZone)
        {
            birdMovement.ResetSpeeds();
        }
    }
}
