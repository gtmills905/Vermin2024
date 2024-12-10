using UnityEngine;
using Photon.Pun;

public class BirdMovement : MonoBehaviourPunCallbacks
{
    public float moveSpeed = 10f; // Forward movement speed
    public float turnSpeed = 50f; // Rotation speed
    public float verticalSpeed = 5f; // Ascend/descend speed
    public Vector3 cameraOffset = new Vector3(0, 2, -5); // Camera offset position
    public float maxPitchAngle = 89f; // Maximum pitch angle to prevent flipping
    public float wingTiltAngle = 30f; // Maximum tilt angle for the wings
    public float tiltSmoothing = 5f; // Smoothing factor for tilting
    public float forwardTiltAngle = 45f; // Permanent forward tilt angle for the mesh

    public Transform birdMesh; // Reference to the bird's mesh object
    public bool inZone = false;
    public PickupControl pickupControl;
    public bool slowBirdsActive1 = false;
    public int maxAnimalsCarried = 1;
    public SlowBirds slowBirds;
    public int maxHealth = 100;

    private int currentHealth;

    public Camera playerCamera;
    private float pitch = 0f; // Tracks current pitch
    private float yaw = 0f; // Tracks current yaw
    private float currentWingTilt = 0f; // Tracks the current tilt of the wings

    private CharacterController characterController; // Reference to the CharacterController
    private Vector3 moveDirection; // Current movement direction
    public float gravity = 9.8f; // Gravity value

    public Transform target; // The player or bird the camera follows
    public float distance = 5f; // Default camera distance from the target
    public float minDistance = 1f; // Minimum distance to avoid clipping
    public float smoothSpeed = 10f; // Speed for smoothing camera transitions
    public LayerMask obstacleLayer; // Layers to consider as obstacles

    private Vector3 desiredPosition;




    void Start()
    {
        UiController.instance.healthSlider.maxValue = maxHealth;
        currentHealth = maxHealth;
        UiController.instance.healthSlider.value = currentHealth;

        playerCamera = Camera.main; // Assign the main camera
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the game screen
        Cursor.visible = false; // Hide the cursor

        characterController = GetComponent<CharacterController>(); // Get the CharacterController
        Transform newTrans = RespawnManager.instance.GetSpawnPoint();
        transform.position = newTrans.position;
        transform.rotation = newTrans.rotation;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            HandleMovement();
            HandleRotation();
            HandleMeshRotation();
            FollowCamera();
        }
    }

    void HandleMovement()
    {
        moveDirection = Vector3.zero;

        // Forward movement
        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += transform.forward * moveSpeed;
        }

        // Ascend (Space) or Descend (S)
        if (Input.GetKey(KeyCode.Space))
        {
            moveDirection += Vector3.up * verticalSpeed;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveDirection += Vector3.down * verticalSpeed;
        }

        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the CharacterController
        characterController.Move(moveDirection * Time.deltaTime);
    }

    void HandleRotation()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Update yaw and pitch based on mouse input
        yaw += mouseX * turnSpeed * Time.deltaTime;
        pitch -= mouseY * turnSpeed * Time.deltaTime;

        // Clamp pitch to prevent flipping upside down
        pitch = Mathf.Clamp(pitch, -maxPitchAngle, maxPitchAngle);

        // Apply rotation to the bird
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        AnimalsControlled();
    }

    public void AnimalsControlled()
    {
        if (!inZone && !(pickupControl != null && pickupControl.animalAttached))
        {
            ResetSpeeds();
        }
        else
        {
            AdjustSpeeds();
        }
    }

    void HandleMeshRotation()
    {
        // Calculate target tilt based on horizontal mouse movement
        float targetWingTilt = -Input.GetAxis("Mouse X") * wingTiltAngle;

        // Smoothly interpolate to the target tilt
        currentWingTilt = Mathf.Lerp(currentWingTilt, targetWingTilt, Time.deltaTime * tiltSmoothing);

        // Apply the forward tilt and wing tilt to the bird's mesh
        if (birdMesh != null)
        {
            birdMesh.localRotation = Quaternion.Euler(forwardTiltAngle, 0f, currentWingTilt);
        }
    }

    void FollowCamera()
    {
        // Base desired position for the camera
        Vector3 desiredPosition = transform.position - transform.forward * cameraOffset.z + Vector3.up * cameraOffset.y;

        // Perform a raycast to check for obstacles
        RaycastHit hit;
        if (Physics.Raycast(transform.position, desiredPosition - transform.position, out hit, cameraOffset.z, LayerMask.GetMask("Ground")))
        {
            // If an obstacle is detected, adjust the desired position
            float hitDistance = Vector3.Distance(transform.position, hit.point);
            desiredPosition = transform.position - transform.forward * Mathf.Max(hitDistance - 0.1f, 1f) + Vector3.up * cameraOffset.y;
        }

        // Smoothly move the camera to the desired position
        playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, desiredPosition, Time.deltaTime * 10f);

        // Match the camera's rotation to the bird's rotation
        playerCamera.transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }



    public void ResetSpeeds()
    {
        verticalSpeed = 5f;
        moveSpeed = 10f;
    }

    public void AdjustSpeeds()
    {
        verticalSpeed = 3f;
        moveSpeed = 7f;
    }

    [PunRPC]
    public void TakeDamage(string damager, int damageAmount)
    {
        if (photonView.IsMine)
        {
            currentHealth -= damageAmount;

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                PlayerSpawner.Instance.Die(damager);
            }
            UiController.instance.healthSlider.value = currentHealth;
        }
    }

    public void DealDamage(string damager, int damageAmount)
    {
        TakeDamage(damager, damageAmount);
    }
}
