using Unity.Netcode;
using UnityEngine;

public class PlayerCameraController : NetworkBehaviour
{
    private Camera playerCamera;

    private void Start()
    {
        // Get the Camera component on this player object
        playerCamera = GetComponentInChildren<Camera>();

        // Enable camera only for the owner
        if (!IsOwner && playerCamera != null)
        {
            playerCamera.enabled = false;
            playerCamera.gameObject.SetActive(false); // Also disable the GameObject for safety
        }
    }
}
