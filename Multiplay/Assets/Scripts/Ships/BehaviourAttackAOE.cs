using System.Collections.Generic;
using UnityEngine;

public class BehaviourAttackAOE : MonoBehaviour, IAttackBehaviour
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
        if (laneController == null || owner == null) return;

        attackTimer += Time.deltaTime;
        if (attackTimer >= attackInterval)
        {
            attackTimer = 0f;
            List<ShipBase> target = laneController.GetEnemiesInAoeRange(owner, attackRangeT);

            foreach (ShipBase ship in target)
            {
                ship.TakeDamage(damage);                
            }
        }
    }

    public bool HasEnemyInRange()
    {
        if (laneController == null || owner == null) return false;
        ShipBase target = laneController.GetClosestEnemyInRange(owner, attackRangeT);
        return target != null;
    }
}

public interface IAttackBehaviour
{
    public bool HasEnemyInRange();
    public void Initialize(LaneController lane);
    public void PerformAttack();
}