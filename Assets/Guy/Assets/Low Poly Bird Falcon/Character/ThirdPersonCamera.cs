using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player; // Reference to the player object
    public CinemachineVirtualCamera mainCamera; // Assign the main virtual camera in the inspector
    public CinemachineVirtualCamera secondaryCamera; // Assign the secondary virtual camera in the inspector

    private float initialYPosition;

    private void Start()
    {
        initialYPosition = player.position.y;
    }

    private void Update()
    {
        // Check if the player's Y position remains the same
        bool isYPositionSame = Mathf.Approximately(player.position.y, initialYPosition);

        // Update camera based on player movement
        if (!isYPositionSame)
        {
            mainCamera.Priority = 10; // Set the priority of the main virtual camera
            secondaryCamera.Priority = 5; // Set the priority of the secondary virtual camera
        }
        else
        {
            mainCamera.Priority = 5; // Set the priority of the main virtual camera
            secondaryCamera.Priority = 10; // Set the priority of the secondary virtual camera
        }
    }
}
