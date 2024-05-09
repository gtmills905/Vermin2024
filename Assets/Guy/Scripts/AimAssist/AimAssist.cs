using UnityEngine;

public class AimAssist : MonoBehaviour
{
    public string targetTag = "Player"; // Tag of the target object
    public float rotationSpeed = 5f; // Speed of camera rotation towards the target
    public Transform player4Camera; // Player 4's camera
    public string triggerAxis = "LT_Player4"; // Axis for left trigger input of player 4

    private Transform target; // Reference to the target object

    void Start()
    {
        // Find the target object by tag
        target = GameObject.FindGameObjectWithTag(targetTag).transform;
    }

    void Update()
    {
        // Check if left trigger of player 4 is pressed
        if (Input.GetAxis(triggerAxis) > 0)
        {
            // Check if target is found
            if (target != null && player4Camera != null)
            {
                // Calculate direction to target
                Vector3 targetDir = target.position - player4Camera.position;
                targetDir.y = 0f;

                // Rotate the camera towards the target smoothly
                Quaternion targetRotation = Quaternion.LookRotation(targetDir);
                player4Camera.rotation = Quaternion.Slerp(player4Camera.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
}
