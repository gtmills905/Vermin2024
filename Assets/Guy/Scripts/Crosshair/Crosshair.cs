using UnityEngine;

public class Crosshair : MonoBehaviour
{
    // Variable to store the crosshair image
    public Texture2D crosshairImage; // The image for the crosshair
    public Camera targetCamera; // The camera to which the crosshair is attached
    [Range(0.1f, 2.0f)] public float crosshairScale = 1.0f; // The scale of the crosshair

    void OnGUI()
    {
        // Draw the crosshair image
        DrawCrosshair();
    }

    void DrawCrosshair()
    {
        if (targetCamera == null)
        {
            Debug.LogWarning("Target camera not assigned for crosshair.");
            return;
        }

        // Calculate the position of the crosshair in screen coordinates
        Vector3 screenPos = targetCamera.WorldToScreenPoint(targetCamera.transform.position + targetCamera.transform.forward * 10f);

        // Calculate scaled width and height of the crosshair
        float scaledWidth = crosshairImage.width * crosshairScale;
        float scaledHeight = crosshairImage.height * crosshairScale;

        // Draw the crosshair image at the calculated position with scaled size
        GUI.DrawTexture(new Rect(screenPos.x - (scaledWidth / 2), Screen.height - screenPos.y - (scaledHeight / 2), scaledWidth, scaledHeight), crosshairImage);
    }
}
