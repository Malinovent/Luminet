using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [SerializeField] private MoveTowardsTarget moveTowardsTarget;
    [SerializeField] private GameObject explosionPrefab;

    private int damage;
    private ShipBase target;

    private void Awake()
    {
        moveTowardsTarget.OnTargetReached += OnTargetReached;
    }

    public void Initialize(Transform target, int damage)
    {
        moveTowardsTarget.SetTarget(target);
        this.damage = damage;

        this.target = target.GetComponent<ShipBase>();
    }

    public void Update()
    {
        if (target == null)
        {
            moveTowardsTarget.ClearTarget();
            moveTowardsTarget.OnTargetReached -= OnTargetReached;
            Destroy(this.gameObject);
            return;
        }

        moveTowardsTarget.Move();
    }

    private void OnTargetReached()
    {
        target?.TakeDamage(damage);
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        moveTowardsTarget.ClearTarget();
        moveTowardsTarget.OnTargetReached -= OnTargetReached;
        Destroy(this.gameObject);
    }

}
