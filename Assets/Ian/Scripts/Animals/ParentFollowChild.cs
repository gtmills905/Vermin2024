using UnityEngine;

public class ParentFollowChild : MonoBehaviour
{
    public Transform child; // Reference to the child transform

    void Update()
    {
        // Ensure child is not null
        if (child != null)
        {
            // Set the parent's position to match the child's position
            transform.position = child.position;
        }
    }
}
