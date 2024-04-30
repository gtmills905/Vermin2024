using UnityEngine;

public class AimAssist : MonoBehaviour
{
    public string targetTag = "Player"; // Tag of the target object
    public float rotationSpeed = 5f; // Speed of camera rotation towards the target
    public float lockDistance = 5f; // Distance at which soft lock occurs
    public float followDistance = 10f; // Distance at which the camera follows the target
    public float followHeight = 5f; // Height at which the camera follows the target
    public Transform player4Camera; // Player 4's camera
    public string triggerAxis = "LT_Player4"; // Axis for left trigger input of player 4

    private Transform target; // Reference to the target object
    private bool isLocked = false; // Flag to indicate if soft lock is active

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

                // Calculate distance to target
                float distanceToTarget = targetDir.magnitude;

                // Soft lock when within lockDistance
                if (distanceToTarget <= lockDistance)
                {
                    isLocked = true;
                }
                else
                {
                    isLocked = false;
                }

                // Rotate the camera towards the target smoothly
                if (!isLocked)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(targetDir);
                    player4Camera.rotation = Quaternion.Slerp(player4Camera.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }

                // Follow the target at a distance when not locked
                Vector3 desiredPosition = target.position - player4Camera.forward * followDistance;
                desiredPosition.y = followHeight;
                player4Camera.position = Vector3.Lerp(player4Camera.position, desiredPosition, Time.deltaTime);
            }
        }
    }
}
