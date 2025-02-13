using UnityEngine;
using Photon.Pun;
using System.Collections;


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
    public int maxHealth = 200;

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

    private bool isRegeneratingHealth = false;


    void Start()
    {
        if (photonView.IsMine)
        {
            currentHealth = maxHealth;
            UiController.instance.healthSlider.maxValue = maxHealth;
            UiController.instance.healthSlider.value = currentHealth;

            playerCamera = Camera.main; // Assign the main camera
            Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the game screen
            Cursor.visible = false; // Hide the cursor

            characterController = GetComponent<CharacterController>(); // Get the CharacterController
            Transform newTrans = RespawnManager.instance.GetSpawnPoint();
            transform.position = newTrans.position;
            transform.rotation = newTrans.rotation;

            StartHealthRegeneration();
        }

    }

    void Update()
    {
        if (photonView.IsMine)
        {
            Application.targetFrameRate = 60;

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
    public void DealDamage(string damager, int damageAmount)
    {
        TakeDamage(damager, damageAmount);
    }

    [PunRPC]
    public void TakeDamage(string damager, int damageAmount)
    {
        if (!photonView.IsMine) return; // Only process damage for the local player

        currentHealth -= damageAmount;
        Debug.Log($"{photonView.Owner.NickName} took {damageAmount} damage. Remaining health: {currentHealth}");

        // Update health slider for the local player
        if (UiController.instance != null && UiController.instance.healthSlider != null)
        {
            UiController.instance.healthSlider.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            photonView.RPC("Die", RpcTarget.All, damager);
        }
    }



    [PunRPC]
    public void Die(string damager)
    { 
        // Notify GameManager that the farmer killed the bird
        GameManager.Instance.FarmerKill(1); // Update kill count for the farmer (the one who killed the bird)

        if (!photonView.IsMine) return; // Only handle death for the local player

        Debug.Log($"{photonView.Owner.NickName} has died!"); // Log death for the dying player

        // Update UI for the dying player only
        UiController.instance.deathText.text = $"{photonView.Owner.NickName} was killed by {damager}";
        UiController.instance.deathScreen.SetActive(true);

        // Spawn death effect
        if (PlayerSpawner.Instance.deathEffect != null)
        {
            PhotonNetwork.Instantiate(PlayerSpawner.Instance.deathEffect.name, transform.position, Quaternion.identity);
        }

        // Notify PlayerSpawner to respawn the player
        PlayerSpawner.Instance.RespawnPlayerAfterDelay();

        // Destroy the player object (dying player only)
        PhotonNetwork.Destroy(gameObject);

       
    }



    [PunRPC]
    public void BroadcastKillToAll(string damager)
    {
        if (photonView.IsMine) return; // Prevent repeating the same event on the local player

        // This is for notifying all clients that a kill has occurred
        GameManager.Instance.FarmerKill(1);  // Update kill count on all clients
    }


    // Coroutine to regenerate health every 2 seconds
    private void StartHealthRegeneration()
    {
        if (isRegeneratingHealth) return; // Avoid starting multiple coroutines

        isRegeneratingHealth = true;
        StartCoroutine(RegenerateHealth());
    }

    // Coroutine that regenerates health
    private IEnumerator RegenerateHealth()
    {
        while (currentHealth < maxHealth)
        {
            yield return new WaitForSeconds(2f); // Wait for 2 seconds

            // Regenerate health by 10 (make sure it doesn't exceed maxHealth)
            currentHealth = Mathf.Min(currentHealth + 10, maxHealth);

            // Update the health slider if it's available
            if (UiController.instance != null && UiController.instance.healthSlider != null)
            {
                UiController.instance.healthSlider.value = currentHealth;
            }

            Debug.Log($"{photonView.Owner.NickName} regenerated 10 health. Current health: {currentHealth}");
        }

        isRegeneratingHealth = false; // Stop regenerating once full health is reached
    }
}
