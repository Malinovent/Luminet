using Unity.Netcode;
using UnityEngine;

public class BehaviourAttackSingle : NetworkBehaviour, IAttackBehaviour
{
    [SerializeField] private float attackRangeT = 0.025f;
    [SerializeField] private float attackInterval = 1f;
    [SerializeField] private int damage = 1;
    [SerializeField] private GameObject projectilePrefab;

    private float attackTimer;
    private ShipBase owner;
    private LaneController laneController;

    private Transform target;

    private void Awake()
    {
        owner = GetComponent<ShipBase>();
    }

    public void Initialize(LaneController lane)
    {
        laneController = lane;
    }

    public void PerformAttack()
    {
        if (laneController == null) return;

        attackTimer += Time.deltaTime;
        if (attackTimer >= attackInterval)
        {
            ShipBase target = laneController.GetClosestEnemyInRange(owner, attackRangeT);
            this.target = target.transform;
           
            ShootProjectileClientRpc();

            attackTimer = 0f;
            
            /*if (target != null)
                target.TakeDamage(damage);*/
        }
    }

    [ClientRpc]
    private void ShootProjectileClientRpc()
    {
        if (target == null) return;

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile projectileComponent = projectile.GetComponent<Projectile>();
        NetworkObject netObj = projectileComponent.GetComponent<NetworkObject>();
        netObj.Spawn();
        if (projectileComponent != null)
        {
            projectileComponent.Initialize(target.transform, damage);
        }
    }

    public bool HasEnemyInRange()
    {
        if (laneController == null) return false;

        ShipBase target = laneController.GetClosestEnemyInRange(owner, attackRangeT);
        return target != null;
    }
}
