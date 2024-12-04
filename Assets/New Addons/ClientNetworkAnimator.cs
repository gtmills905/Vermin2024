using UnityEngine;
using Unity.Netcode.Components;

[DisallowMultipleComponent] // Fixed typo: DisallowMutltipleComponent
public class ClientNetworkAnimator : NetworkAnimator
{
    /// <summary>
    /// Overrides the authority model to make this transform client-authoritative.
    /// </summary>
    protected override bool OnIsServerAuthoritative()
    {
        return false; // Returning false makes this component client-authoritative.
    }
}
