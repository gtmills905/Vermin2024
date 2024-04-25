using UnityEngine;

public class CollisionEnlarger : MonoBehaviour
{
    public float triggerScaleMultiplier = 1.5f; // You can adjust this scale multiplier to control the size of the trigger collider

    void Start()
    {
        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>(); // You can replace BoxCollider with the type of collider you need
        boxCollider.isTrigger = true;

        Vector3 originalSize = boxCollider.size;
        boxCollider.size = new Vector3(originalSize.x * triggerScaleMultiplier, originalSize.y * triggerScaleMultiplier, originalSize.z * triggerScaleMultiplier);
    }

    void OnTriggerEnter(Collider other)
    {
        // Custom logic when the trigger is hit by the player or another object
        Debug.Log("Collision detected with the enlarged trigger!");
    }
}
