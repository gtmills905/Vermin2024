using System.Configuration;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCamera; // The original camera
    public Camera alternateCamera; // The camera to switch to
    public SC_FPSController sC_FPSController;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("LT_Player4") > 0) // Assuming "LT" refers to the left trigger button for player 4
        {
            // If left trigger is pressed, switch to alternate camera
            mainCamera.enabled = false;
            alternateCamera.enabled = true;
            sC_FPSController.lookSpeed = 2f;
        }
        else
        {
            // If left trigger is not pressed, revert to original camera
            mainCamera.enabled = true;
            alternateCamera.enabled = false;
            sC_FPSController.lookSpeed = 4f;
        }
    }
}
