using UnityEngine;
using Unity.Netcode;

public class ShipBomb : ShipBase
{
    [SerializeField] int playerDamage = 10;
    [SerializeField] BehaviourCapturePoint capturePointBehaviour;    

    public override void Initialize(LaneController laneController, int moveDirection, ulong ownerID)
    {
        base.Initialize(laneController, moveDirection, ownerID);
        capturePointBehaviour.Initialize(ownerID);
    }

    protected override void OnPathEndReached()
    {
        DamagePlayer();
    }

    private void DamagePlayer()
    {
        NetworkObject targetPlayer = NetworkManager.Singleton.ConnectedClients[laneController.GetClientId(direction)].PlayerObject;
        PlayerBase playerBase = targetPlayer.GetComponent<PlayerBase>();
        if (playerBase != null)
        {            
            playerBase.TakeDamage(playerDamage);
            Die();
        }
    }

    private void Update()
    {
        if (!IsServer) return;

        MoveAlongSpline();        
    }

}
