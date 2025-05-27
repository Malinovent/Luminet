using Unity.Netcode;
using UnityEngine;

public class PlayerBase : NetworkBehaviour
{
    [SerializeField] private Health3D health;

    public void TakeDamage(int amount)
    {
        health.TakeDamage(amount);
    }

    public void Die()
    {
        if (!IsServer) return;

        ShowWinLoseScreenClientRpc();
    }

    [ClientRpc]
    private void ShowWinLoseScreenClientRpc()
    {
        if (IsOwner)
        {
            UIManager.Instance.LoseGameUI();
        }
        else
        {
            UIManager.Instance.WinGameUI();
        }
    }
}
