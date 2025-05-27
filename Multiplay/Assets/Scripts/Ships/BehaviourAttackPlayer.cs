using Unity.Netcode;
using UnityEngine;

public class BehaviourAttackPlayer : MonoBehaviour
{
    [SerializeField] private float attackInterval = 1f;
    [SerializeField] private int damage = 1;

    private float attackTimer;
    private LaneController laneController;
    private int direction = 0;

    public void Initialize(LaneController lane, int direction)
    {
        laneController = lane;
        this.direction = direction;
    }

    public void PerformAttack()
    {
        if (laneController == null) return;

        attackTimer += Time.deltaTime;
        if (attackTimer >= attackInterval)
        {
            attackTimer = 0f;
            DamagePlayer();
        }
    }

    private void DamagePlayer()
    {
        NetworkObject targetPlayer = NetworkManager.Singleton.ConnectedClients[laneController.GetClientId(direction)].PlayerObject;
        PlayerBase playerBase = targetPlayer.GetComponent<PlayerBase>();
        if (playerBase != null)
        {
            playerBase.TakeDamage(damage);            
        }
    }
}
