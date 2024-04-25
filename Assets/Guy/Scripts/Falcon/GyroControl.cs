using UnityEngine;

public class GyroControl : MonoBehaviour
{
    public float verticalRotationSpeed = 0.1f; // Adjust this to control the strength of the vertical gyro effect
    public float horizontalRotationSpeed = 0.1f; // Adjust this to control the strength of the horizontal gyro effect
    public float maxRotationAngle = 80f; // Adjust this to limit the maximum rotation angle on the x-axis
    private Quaternion initialRotation;
    private bool isMoving = false;

    private void Start()
    {
        initialRotation = transform.rotation;
    }

    private void Update()
    {
        // Check if the player is providing any input for movement
        isMoving = Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0;

        // Apply the gyro effect only when the player is moving
        if (isMoving)
        {
            // Get the current rotation of the object
            Quaternion currentRotation = transform.rotation;

            // Calculate the difference between the current and initial rotation
            Quaternion deltaRotation = Quaternion.Inverse(initialRotation) * currentRotation;

            // Convert the delta rotation to euler angles
            Vector3 deltaEulerAngles = deltaRotation.eulerAngles;

            // Ensure the x-axis rotation does not exceed the maximum angle
            float clampedAngleX = Mathf.Clamp(deltaEulerAngles.x, -maxRotationAngle, maxRotationAngle);
            deltaEulerAngles.x = clampedAngleX;

            // Convert the clamped delta euler angles back to a quaternion
            Quaternion clampedDeltaRotation = Quaternion.Euler(deltaEulerAngles);

            // Apply a smooth transition to the rotation based on the clamped delta euler angles
            float verticalRotationStep = verticalRotationSpeed * Time.deltaTime;
            float horizontalRotationStep = horizontalRotationSpeed * Time.deltaTime;

            Quaternion verticalRotation = Quaternion.Slerp(transform.rotation, initialRotation * Quaternion.Euler(clampedDeltaRotation.eulerAngles.x, 0, 0), verticalRotationStep);
            Quaternion horizontalRotation = Quaternion.Slerp(verticalRotation, initialRotation * Quaternion.Euler(0, clampedDeltaRotation.eulerAngles.y, 0), horizontalRotationStep);

            transform.rotation = horizontalRotation;
        }
        else
        {
            // Reset the rotation to ensure the object remains upright when the player is not moving
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }
    }
}
