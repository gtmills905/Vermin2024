using UnityEngine;
using Photon.Pun;
using UnityEngine.Rendering;

public class MeshInvisible : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer[] headMeshes; // Assign multiple meshes in Inspector
    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();

        if (photonView.IsMine && IsFarmer()) // If this is the local player's farmer
        {
            SetHeadToShadowsOnly(true);
        }
    }

    private void SetHeadToShadowsOnly(bool enableShadows)
    {
        if (headMeshes != null)
        {
            foreach (SkinnedMeshRenderer mesh in headMeshes)
            {
                if (mesh != null)
                {
                    mesh.shadowCastingMode = enableShadows ? ShadowCastingMode.ShadowsOnly : ShadowCastingMode.On;
                }
            }
        }
    }

    private bool IsFarmer()
    {
        // Replace this with actual logic to determine if this player is the farmer
        return gameObject.CompareTag("Farmer");
    }
}
