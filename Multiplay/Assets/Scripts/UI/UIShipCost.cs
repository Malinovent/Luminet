using UnityEngine;
using TMPro;
using System;

public class UIShipCost : MonoBehaviour
{
    [SerializeField] private TMP_Text costText;
    [SerializeField] private ShipData ship;

    public void Initialize(PlayerScore playerScore)
    {
        playerScore.Resource.OnValueChanged += (oldVal, newVal) => UpdateCostUI(newVal);
        UpdateCostUI(playerScore.Resource.Value);
    }

    private void UpdateCostUI(int val)
    {
        if (val < ship.cost)
        {
            costText.text = $"<color=red>{ship.cost}</color>";
        }
        else
        {
            costText.text = $"<color=black>{ship.cost}</color>"; 
        }

    }
}
