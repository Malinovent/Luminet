using Unity.Netcode;
using UnityEngine;

public class BehaviourAttackSingle : NetworkBehaviour, IAttackBehaviour
{
    [SerializeField] private float attackRangeT = 0.025f;
    [SerializeField] private float attackInterval = 1f;
    [SerializeField] private int damage = 1;

    private float attackTimer;
    private ShipBase owner;
    private LaneController laneController;

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
            attackTimer = 0f;
            ShipBase target = laneController.GetClosestEnemyInRange(owner, attackRangeT);
            if (target != null)
                target.TakeDamage(damage);
        }
    }    

    public bool HasEnemyInRange()
    {
        if (laneController == null) return false;

        ShipBase target = laneController.GetClosestEnemyInRange(owner, attackRangeT);
        return target != null;
    }
}
