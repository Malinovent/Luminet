using Unity.Netcode;
using UnityEngine;

public class PlayerBase : NetworkBehaviour
{
    [SerializeField] private Health3D health;

    public void TakeDamage(int amount)
    {
        health.TakeDamage(amount);
    }
}
