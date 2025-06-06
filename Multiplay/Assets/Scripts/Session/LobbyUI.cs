using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] TMP_Text playersText;
    [SerializeField] GameObject playerNamePrefab;
    [SerializeField] Transform playersListParent; // Assign in inspector

    private List<GameObject> playerEntries = new List<GameObject>();

    public void UpdatePlayers(Lobby lobby)
    {
        foreach(GameObject entry in playerEntries)
        {
            Destroy(entry);
        }

        playerEntries.Clear();

        foreach (Player player in lobby.Players)
        {
            GameObject entry = Instantiate(playerNamePrefab, playersListParent);
            ActiveLobbyPlayer nameText = entry.GetComponent<ActiveLobbyPlayer>();

            string nameString = player.Id; // Default fallback

            if (player.Data != null && player.Data.ContainsKey("DisplayName") && player.Data["DisplayName"] != null)
            {
                // Use display name if set
                nameString = player.Data["DisplayName"].Value;
            }

            if (nameText != null)
                nameText.UpdatePlayerName(nameString);

            playerEntries.Add(entry);
        }

    }
}
