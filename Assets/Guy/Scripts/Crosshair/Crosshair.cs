using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    // Reference to the image component of the crosshair
    public Image crosshairImage;

    // Reference to the main camera
    public Camera mainCamera;

    void Start()
    {

        // Get the main camera
        mainCamera = Camera.main;

        // Set the crosshair image to the center of the camera's view
        SetCrosshairToCenter();
    }

    void Update()
    {
        // Update crosshair position every frame in case the camera moves
        SetCrosshairToCenter();
    }

    void SetCrosshairToCenter()
    {
        // Get the center of the camera's view
        Vector3 cameraCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);

        // Convert the center of the camera's view from screen coordinates to world coordinates
        Vector3 worldCenter = mainCamera.ScreenToWorldPoint(cameraCenter);

        // Make sure the crosshair is visible within the camera's view
        float distanceFromCamera = Vector3.Distance(mainCamera.transform.position, worldCenter);
        Vector3 normalizedViewportCenter = mainCamera.WorldToViewportPoint(worldCenter);

        // Check if the world center is within the camera's view
        if (normalizedViewportCenter.x >= 0f && normalizedViewportCenter.x <= 1f &&
            normalizedViewportCenter.y >= 0f && normalizedViewportCenter.y <= 1f &&
            normalizedViewportCenter.z >= 0f && distanceFromCamera > 0f)
        {
            // Set the crosshair's position to the world center
            crosshairImage.rectTransform.position = worldCenter;
            crosshairImage.enabled = true; // Ensure the crosshair is enabled
        }
    }
}
