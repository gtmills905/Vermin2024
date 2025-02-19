using Photon.Pun;

public class PlayerLeaveHandler : MonoBehaviourPunCallbacks
{
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        // Check if it's the local player and destroy their game object
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);  // Destroys the local player's object
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        // Optional: Handle when other players leave, but make sure you destroy the local player's object
        if (otherPlayer.IsLocal)
        {
            PhotonNetwork.Destroy(gameObject);  // Destroys the local player's object
        }
    }
}
