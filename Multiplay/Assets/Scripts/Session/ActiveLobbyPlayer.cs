using TMPro;
using UnityEngine;

public class ActiveLobbyPlayer : MonoBehaviour
{
    [SerializeField] private TMP_Text playerName;
    
    public void UpdatePlayerName(string name)
    {
        if (playerName != null)
        {
            playerName.text = name;
        }
        else
        {
            Debug.LogWarning("PlayerName TMP_Text is not assigned.");
        }
    }
}
