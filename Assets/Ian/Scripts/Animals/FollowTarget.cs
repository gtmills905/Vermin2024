using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target; // The target to follow
    public float followDistance = 2f; // The distance to maintain from the target
    public float followSpeed = 3f; // The speed at which to follow the target
    public float rotationSpeed = 5f; // The speed at which to rotate towards the target

    private void Update()
    {
        if (target == null) return;

        // Calculate the desired position
        Vector3 desiredPosition = target.position - target.forward * followDistance;

        // Smoothly move to the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Smoothly rotate to face the target
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }
}
