using Unity.Services.Lobbies.Models;
using UnityEngine;
using TMPro;

public class LobbyContainerUI : MonoBehaviour
{
    [SerializeField] TMP_Text lobbyNameText;
    [SerializeField] GameObject privateIcon;
    [SerializeField] TMP_Text playerCountText;
    [SerializeField] GameObject joinButton;

    private Lobby lobby;
    private bool hasPassword = false;

    public void SetLobbyData(Lobby lobbyData)
    {
        Debug.Log($"Lobby Name: {lobbyData.Name}, LobbyCode: {lobbyData.LobbyCode}, Id: {lobbyData.Id}");
        lobbyNameText.text = lobbyData.Name;

        hasPassword = lobbyData.Data.TryGetValue("IsPrivate", out var isPrivateData)
                  && bool.TryParse(isPrivateData.Value, out bool isPrivate)
                  && isPrivate;

        privateIcon.SetActive(hasPassword);
        playerCountText.text = $"{lobbyData.Players.Count}/{lobbyData.MaxPlayers}";
        joinButton.SetActive(lobbyData.Players.Count < lobbyData.MaxPlayers);

        lobby = lobbyData;
    }

    public void TryJoin()
    {
        if(hasPassword)
        {
            //Open password UI
            LobbyManager.Instance.OpenInputPassword();
            Debug.Log("Opening password input for private lobby.");
        }
        else
        {
            Debug.Log("Joining public lobby without password.");
            int nextPlayerNumber = lobby.Players.Count + 1;
            string displayName = $"Player {nextPlayerNumber}";

            JoinPublicLobby(displayName);
        }
    }

    private async void JoinPublicLobby(string displayName)
    {
        try
        {
            await LobbyManager.Instance.JoinLobbyByIdAsync(lobby.Id, displayName);
            LobbyManager.Instance.OpenLobby(lobby);
            Debug.Log("Joined lobby successfully.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to join lobby: {ex.Message}");
        }
    }
}
