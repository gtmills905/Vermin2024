using UnityEngine;

public class ActivateAndReplaceOnCollision : MonoBehaviour
{
    [SerializeField] private GameObject objectToActivate;
    [SerializeField] private GameObject replacementObject;
    [SerializeField] private float collisionForceThreshold = 10f;
    [SerializeField] private string groundTag = "Ground";

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(groundTag))
        {
            if (collision.relativeVelocity.magnitude > collisionForceThreshold)
            {
                if (objectToActivate != null && !objectToActivate.activeSelf)
                {
                    objectToActivate.SetActive(true);
                    if (replacementObject != null)
                    {
                        Instantiate(replacementObject, objectToActivate.transform.position, objectToActivate.transform.rotation);
                        Destroy(objectToActivate);
                    }
                }
            }
        }
    }
}
