using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] TMP_Text playersText;
    [SerializeField] TMP_Text lobbyCodeText;
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
            ActiveLobbyPlayer playerUI = entry.GetComponent<ActiveLobbyPlayer>();

            string nameString = player.Id; // Default fallback

            if (player.Data != null && player.Data.ContainsKey("DisplayName") && player.Data["DisplayName"] != null)
            {
                // Use display name if set
                nameString = player.Data["DisplayName"].Value;
            }

            if (playerUI != null)
                playerUI.SetPlayer(nameString, player.Id, lobby.Id, lobby.HostId);
            
            //playerUI.UpdatePlayerName(nameString);

            playerEntries.Add(entry);
        }

        playersText.text = $"Players {lobby.Players.Count}/{lobby.MaxPlayers}";

        UpdateLobbyDisplay(lobby);
    }

    void UpdateLobbyDisplay(Lobby lobby)
    {
        lobbyCodeText.text = $"Code: {lobby.LobbyCode}";
    }
}
