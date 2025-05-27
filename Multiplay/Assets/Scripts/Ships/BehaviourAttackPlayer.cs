using Unity.Netcode;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BehaviourAttackPlayer : MonoBehaviour
{
    [SerializeField] private float attackInterval = 1f;
    [SerializeField] private int damage = 1;
    [SerializeField] private GameObject projectilePrefab;

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

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile projectileComponent = projectile.GetComponent<Projectile>();
        NetworkObject netObj = projectileComponent.GetComponent<NetworkObject>();
        netObj.Spawn();
        if (projectileComponent != null)
        {
            projectileComponent.Initialize(playerBase.transform, damage);
        }
    }
}
