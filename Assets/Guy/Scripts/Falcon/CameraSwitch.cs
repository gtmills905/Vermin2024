using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public Transform player; // Reference to the player object
    public Camera mainCamera; // Assign the main camera in the inspector

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
            mainCamera.depth = 1; // Set the depth of the main camera
        }
        else
        {
            mainCamera.depth = 0; // Set the depth of the main camera
        }
    }
}
