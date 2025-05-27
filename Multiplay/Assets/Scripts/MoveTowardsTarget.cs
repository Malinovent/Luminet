using System;
using UnityEngine;

public class MoveTowardsTarget : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float distanceThreshold = 0.1f;

    public Action OnTargetReached;

    private Transform target;
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void Move()
    {
        if (target == null) return;

        Vector3 direction = (target.position - transform.position).normalized;
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        transform.rotation = Quaternion.LookRotation(direction);

        if (Vector3.Distance(transform.position, target.position) < distanceThreshold)
        {
            OnTargetReached?.Invoke();            
        }
    }

    public void ClearTarget()
    {
        target = null;
    }
}
