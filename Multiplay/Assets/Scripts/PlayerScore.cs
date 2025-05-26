using Mali.Utils;
using Unity.Netcode;
using UnityEngine;

public class PlayerScore : NetworkBehaviour
{
    [SerializeField] private float incomeTime = 10;

    public NetworkVariable<int> Resource = new NetworkVariable<int>();
    public NetworkVariable<int> Income = new NetworkVariable<int>();

    private Timer incomeTimer;

    private void Start()
    {
        incomeTimer = new Timer(incomeTime);
        incomeTimer.OnTimerEnd += GainIncome;
    }

    private void Update()
    {
        if (!IsServer) return;

        incomeTimer.Update(Time.deltaTime);
    }

    public void GainIncome()
    {
        Resource.Value += Income.Value;
        incomeTimer.Reset();
    }

    [ServerRpc]
    public void RequestBuyShipServerRpc(int cost, ServerRpcParams rpcParams = default)
    {
        if (Resource.Value >= cost)
        {
            Resource.Value -= cost;
        }
    }
}
