using UnityEngine;

public class PlaceObjectOnGround : MonoBehaviour
{
    public GameObject objectToPlace; // The object to be placed
    public string groundTag = "Ground"; // The tag of the ground objects
    public float maxDistance = 10f; // Maximum distance to place the object
    public KeyCode placeKey = KeyCode.Joystick4Button0; // Button X on Xbox controller
    public Camera playerCamera; // Reference to the player's camera
    public float cooldownDuration = 60f; // Cooldown duration in seconds

    public GameObject uiScarecrow;


    private float lastPlacementTime; // Time of the last placement

    private void Start()
    {

        uiScarecrow.SetActive(true);
        // Initialize last placement time to ensure the object can be placed immediately
        lastPlacementTime = -cooldownDuration;
    }

    // Update is called once per frame
    void Update()
    {

        if (Time.time - lastPlacementTime >= cooldownDuration)
        {
            {
                uiScarecrow.SetActive(true);
            }
        }
        if (Input.GetKeyDown(placeKey) && Time.time - lastPlacementTime >= cooldownDuration)
        {
            

            // Get the direction in which the player is aiming
            Vector3 aimDirection = GetAimDirection();

            // Raycast from the camera to find the ground
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, aimDirection, out hit, maxDistance))
            {
                if (hit.collider.CompareTag(groundTag))
                {
                    // Place the object at the hit point
                    Instantiate(objectToPlace, hit.point, Quaternion.identity);
                    // Update the last placement time
                    lastPlacementTime = Time.time;

                    uiScarecrow.SetActive(false);
                }
            }
        }
    }

    // Function to get the aim direction from the player's camera
    Vector3 GetAimDirection()
    {
        // Get the camera's forward direction
        Vector3 direction = playerCamera.transform.forward;
        return direction;
    }
}
