using Unity.Netcode;
using UnityEngine;

public class PlayerVisuals : NetworkBehaviour
{
    [SerializeField] private Renderer rend;   

    public void SetClientID(ulong ownerID)
    {
        if (rend != null)
        {
            rend.material.color = PlayerColors.GetColorForClient(ownerID);
            //Debug.Log($"Client ID set to {ownerID}. Setting color to {rend.material.color}");
        }
    }
   
}
