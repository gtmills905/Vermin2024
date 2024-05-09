using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera firstCamera;
    public Camera secondCamera;

    private bool isSwitching = false; // Flag to track if a switch is in progress

    void Update()
    {
        if (Input.GetAxis("LT_Player4") > 0)
        {
            // Only switch cameras if a switch is not already in progress
            if (!isSwitching)
            {
                isSwitching = true; // Set the flag to indicate a switch is in progress

                // If the second camera is active, switch to the first camera; otherwise, switch to the second camera
                if (secondCamera.depth > firstCamera.depth)
                {
                    SwitchToFirstCamera();
                }
                else
                {
                    SwitchToSecondCamera();
                }
            }
        }
        else
        {
            isSwitching = false; // Reset the flag when the button is released
        }
    }

    private void SwitchToSecondCamera()
    {
        firstCamera.depth = 0; // Set the depth of the first camera to a lower value
        secondCamera.depth = 1; // Set the depth of the second camera to a higher value
    }

    private void SwitchToFirstCamera()
    {
        firstCamera.depth = 1; // Set the depth of the first camera to a higher value
        secondCamera.depth = 0; // Set the depth of the second camera to a lower value
    }
}
