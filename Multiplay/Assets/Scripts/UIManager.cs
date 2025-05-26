using Mali.Utils;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    [Header("UI Elements")]
    public TMP_Text resourceText;
    public TMP_Text incomeText;

    PlayerScore localPlayerScore;

    void Start()
    {
        // Wait until player objects are spawned before finding your PlayerScore
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        // Only do this for the local player!
        if (clientId != NetworkManager.Singleton.LocalClientId) return;

        // Find our own PlayerScore object
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.ClientId == clientId)
            {
                localPlayerScore = client.PlayerObject.GetComponent<PlayerScore>();

                // Subscribe to changes
                localPlayerScore.Resource.OnValueChanged += (oldVal, newVal) => UpdateResourceUI(newVal);
                localPlayerScore.Income.OnValueChanged += (oldVal, newVal) => UpdateIncomeUI(newVal);

                // Initialize UI
                UpdateResourceUI(localPlayerScore.Resource.Value);
                UpdateIncomeUI(localPlayerScore.Income.Value);

                break;
            }
        }
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
        if (localPlayerScore != null)
        {                        
            localPlayerScore.RequestBuyShipServerRpc(shipData.cost);
        }
    }
}
