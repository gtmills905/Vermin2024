using UnityEngine;

public class RagdollController : MonoBehaviour
{
    // Reference to the Animator component
    private Animator animator;

    // Flag to determine if the ragdoll is active
    private bool isRagdollActive = false;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Animator component attached to the same GameObject
        animator = GetComponent<Animator>();

        // Disable the ragdoll at the beginning
        SetRagdollActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Example: Implement a key press to toggle the ragdoll on and off (for testing purposes)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isRagdollActive = !isRagdollActive;
            SetRagdollActive(isRagdollActive);
        }
    }

    // Function to enable or disable the ragdoll
    private void SetRagdollActive(bool isActive)
    {
        // Enable or disable the Animator component
        animator.enabled = !isActive;

        // Get an array of all Rigidbody components in the GameObject and its children
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        // Loop through the array and enable or disable each Rigidbody component
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = !isActive;
        }
    }
}