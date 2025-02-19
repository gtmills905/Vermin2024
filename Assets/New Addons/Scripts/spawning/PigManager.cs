using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class PigManager : MonoBehaviourPunCallbacks
{
    private static List<GameObject> allPigs = new List<GameObject>();

    // Register a pig to the manager
    public static void RegisterPig(GameObject pig)
    {
        if (!allPigs.Contains(pig))
        {
            allPigs.Add(pig);
            Debug.Log($"Pig {pig.name} registered.");
        }
    }

    // Unregister a pig from the manager
    public static void UnregisterPig(GameObject pig)
    {
        if (allPigs.Contains(pig))
        {
            allPigs.Remove(pig);
            Debug.Log($"Pig {pig.name} unregistered.");
        }
    }

    // Get all pigs currently in the game
    public static List<GameObject> GetAllPigs()
    {
        return allPigs;
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        List<GameObject> pigsToDestroy = new List<GameObject>();

        // Collect pigs owned by the player who left
        foreach (var pig in allPigs)
        {
            if (pig == null)
            {
                continue;  // Skip null (already destroyed) pigs
            }

            PhotonView pigView = pig.GetComponent<PhotonView>();
            if (pigView != null && pigView.Owner == otherPlayer)
            {
                pigsToDestroy.Add(pig);
            }
        }

        // Destroy and clean up the pigs
        foreach (var pig in pigsToDestroy)
        {
            if (pig != null)  // Double check before destroying
            {
                PhotonNetwork.Destroy(pig);
                allPigs.Remove(pig);
            }
        }
    }

}

