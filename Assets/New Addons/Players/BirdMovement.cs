using System.Security.Cryptography;
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


    public Camera playerCamera;
    private float pitch = 0f; // Tracks current pitch
    private float yaw = 0f; // Tracks current yaw
    private float currentWingTilt = 0f; // Tracks the current tilt of the wings

    void Start()
    {
        playerCamera = Camera.main; // Assign the main camera
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the game screen
        Cursor.visible = false; // Hide the cursor

        Transform newTrans = RespawnManager.instance.GetSpawnPoint();
        transform.position = newTrans.position;
        transform.rotation = newTrans.rotation;
    }



    void Update()
    {

        
        HandleMovement();
        HandleRotation();
        HandleMeshRotation();
        FollowCamera();
        
        
    }

    void HandleMovement()
    {
        Vector3 movement = Vector3.zero;

        // Forward movement
        if (Input.GetKey(KeyCode.W))
        {
            movement += transform.forward * moveSpeed * Time.deltaTime;
        }

        // Ascend (Space) or Descend (S)
        if (Input.GetKey(KeyCode.Space))
        {
            movement += Vector3.up * verticalSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            movement += Vector3.down * verticalSpeed * Time.deltaTime;
        }

        // Apply movement
        transform.position += movement;
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
        // Calculate the desired camera position behind the bird
        Vector3 targetPosition = transform.position - transform.forward * cameraOffset.z + Vector3.up * cameraOffset.y;
        playerCamera.transform.position = targetPosition;

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


}
