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

    // Called when a player leaves the room to destroy pigs they own
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        List<GameObject> pigsToDestroy = new List<GameObject>();
        foreach (var pig in allPigs)
        {
            PhotonView pigView = pig.GetComponent<PhotonView>();
            if (pigView != null && pigView.Owner == otherPlayer)
            {
                pigsToDestroy.Add(pig);
            }
        }

        foreach (var pig in pigsToDestroy)
        {
            PhotonNetwork.Destroy(pig);  // Destroys the pig across all clients
            allPigs.Remove(pig);
        }
    }
}

