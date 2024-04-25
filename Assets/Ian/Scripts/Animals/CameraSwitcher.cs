using UnityEngine;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    public CinemachineVirtualCamera firstCamera;
    public CinemachineVirtualCamera secondCamera;
    public GameObject objectToTrack; // The GameObject whose position you're tracking

    private float lastYPosition;

    void Start()
    {
        // Initialize the lastYPosition variable
        lastYPosition = objectToTrack.transform.position.y;
    }

    void Update()
    {
        // Check if the object's y position has changed
        if (objectToTrack.transform.position.y != lastYPosition)
        {
            // If the second camera is active, switch to the first camera; otherwise, switch to the second camera
            if (secondCamera.Priority > 0)
            {
                SwitchToFirstCamera();
            }
            else
            {
                SwitchToSecondCamera();
            }
        }

        // Update the lastYPosition variable
        lastYPosition = objectToTrack.transform.position.y;
    }

    private void SwitchToSecondCamera()
    {
        firstCamera.Priority = 0;
        secondCamera.Priority = 10;
    }

    private void SwitchToFirstCamera()
    {
        firstCamera.Priority = 10;
        secondCamera.Priority = 0;
    }
}
