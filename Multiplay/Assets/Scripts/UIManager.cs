using Mali.Utils;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Multiplayer.Center.NetcodeForGameObjectsExample.DistributedAuthority;

public class UIManager : Singleton<UIManager>
{
    [Header("UI Elements")]
    public TMP_Text resourceText;
    public TMP_Text incomeText;

    PlayerScore localPlayerScore;
    PlayerShipSpawner localPlayerSpawner;

    public void SetLocalPlayerSpawner(PlayerShipSpawner player)
    {
        if (!player.IsOwner) return;

        localPlayerSpawner = player;
        localPlayerScore = player.GetComponent<PlayerScore>();

        localPlayerScore.Resource.OnValueChanged += (oldVal, newVal) => UpdateResourceUI(newVal);
        localPlayerScore.Income.OnValueChanged += (oldVal, newVal) => UpdateIncomeUI(newVal);

        UpdateResourceUI(localPlayerScore.Resource.Value);
        UpdateIncomeUI(localPlayerScore.Income.Value);

        //Debug.Log($"UIManager assigned localPlayerSpawner: {localPlayerSpawner.OwnerClientId}, LocalClientId: {NetworkManager.Singleton.LocalClientId}");
    }   

    private void UpdateResourceUI(int newVal)
    {
        if (resourceText != null) resourceText.text = $"{newVal}";
    }

    private void UpdateIncomeUI(int newVal)
    {
        if (incomeText != null) incomeText.text = $"{newVal}";
    }

    public void TryBuyShip(ShipData shipData)
    {
        if (localPlayerSpawner != null)
        {
            //Debug.Log($"UIManager calling spawn: LocalClientId: {NetworkManager.Singleton.LocalClientId}, Spawner Owner: {localPlayerSpawner.OwnerClientId}");
            //localPlayerScore.RequestBuyShipServerRpc(shipData.shipIndex, shipData.cost);
            localPlayerSpawner.RequestSpawnShipServerRpc(shipData.shipIndex, shipData.cost, LaneManager.Instance.GetSelectedLaneIndex());
            //localPlayerSpawner.RequestSpawnShipServerRpc(shipData.shipIndex);
        }
    }

    private void OnDestroy()
    {
        if (localPlayerScore != null)
        {
            localPlayerScore.Resource.OnValueChanged -= (oldVal, newVal) => UpdateResourceUI(newVal);
            localPlayerScore.Income.OnValueChanged -= (oldVal, newVal) => UpdateIncomeUI(newVal);
        }
    }
}
