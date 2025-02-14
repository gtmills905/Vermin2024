using UnityEngine;
using Photon.Pun;


public class ArmsFollowMousePhoton : MonoBehaviour
{
    private Transform leftArm;
    private Transform rightArm;
    private Camera mainCamera;
    private PhotonView photonView;

    void Start()
    {
        // Find the main camera dynamically
        mainCamera = Camera.main;

        // Get the PhotonView component
        photonView = GetComponent<PhotonView>();

        // Find arms dynamically
        leftArm = transform.Find("mixamorig:LeftArm");
        rightArm = transform.Find("mixamorig:RightArm");

        if (leftArm == null || rightArm == null)
        {
            Debug.LogWarning("Arms not found! Ensure they are children of the player prefab.");
        }
    }

    void Update()
    {
        if (!photonView.IsMine || mainCamera == null) return; // Only local player controls arms

        // Get mouse position in world space
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 targetPosition = hit.point;
            Vector3 direction = targetPosition - transform.position;

            // Calculate rotation
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            float xRotation = lookRotation.eulerAngles.x;

            // Rotate arms separately
            if (leftArm != null) leftArm.rotation = Quaternion.Euler(xRotation, transform.eulerAngles.y, 0);
            if (rightArm != null) rightArm.rotation = Quaternion.Euler(xRotation, transform.eulerAngles.y, 0);

            // Sync rotation over the network
            photonView.RPC("SyncArmsRotation", RpcTarget.Others, xRotation);
        }
    }

    [PunRPC]
    void SyncArmsRotation(float xRotation)
    {
        if (photonView.IsMine) return; // Don't sync for the local player
        if (leftArm != null) leftArm.rotation = Quaternion.Euler(xRotation, transform.eulerAngles.y, 0);
        if (rightArm != null) rightArm.rotation = Quaternion.Euler(xRotation, transform.eulerAngles.y, 0);
    }
}
