using UnityEngine;

public class PlaceObjectOnGround : MonoBehaviour
{
    public GameObject objectToPlace; // The object to be placed
    public string groundTag = "Ground"; // The tag of the ground objects
    public float maxDistance = 10f; // Maximum distance to place the object
    public KeyCode placeKey = KeyCode.Joystick4Button0; // Button X on Xbox controller

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(placeKey))
        {
            // Get the direction in which the player 4 controller is aiming
            Vector3 aimDirection = GetAimDirection();

            // Raycast to find the ground
            RaycastHit hit;
            if (Physics.Raycast(transform.position, aimDirection, out hit, maxDistance))
            {
                if (hit.collider.CompareTag(groundTag))
                {
                    // Place the object at the hit point
                    Instantiate(objectToPlace, hit.point, Quaternion.identity);
                }
            }
        }
    }

    // Function to get the aim direction from the player 4 controller
    Vector3 GetAimDirection()
    {
        float horizontal = Input.GetAxis("RightJoystickHorizontal4");
        float vertical = Input.GetAxis("RightJoystick4Vertica4l");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        return direction;
    }
}
