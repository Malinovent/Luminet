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

    public void SetLobbyData(Lobby lobbyData)
    {
        Debug.Log($"Lobby Name: {lobbyData.Name}, LobbyCode: {lobbyData.LobbyCode}, Id: {lobbyData.Id}");
        lobbyNameText.text = lobbyData.Name;
        privateIcon.SetActive(lobbyData.IsPrivate);
        playerCountText.text = $"{lobbyData.Players.Count}/{lobbyData.MaxPlayers}";
        joinButton.SetActive(lobbyData.Players.Count < lobbyData.MaxPlayers);

        lobby = lobbyData;
    }

    public void TryJoin()
    {
        if(lobby.IsPrivate)
        {
            //Open password UI
        }
        else
        {
            int nextPlayerNumber = lobby.Players.Count + 1;
            string displayName = $"Player {nextPlayerNumber}";

            JoinPublicLobby(displayName);
        }
    }

    private async void JoinPublicLobby(string displayName)
    {
        try
        {
            /*if (string.IsNullOrWhiteSpace(lobby.LobbyCode))
            {
                Debug.LogError("LobbyCode is null or empty. Cannot join lobby.");
                return;
            }*/

            int randomNameSuffix = Random.Range(1, 9999);
            //LobbyManager.Instance.JoinLobbyByCodeAsync(lobby.LobbyCode, $"RandomPerson{randomNameSuffix}");
            await LobbyManager.Instance.JoinLobbyByIdAsync(lobby.Id, displayName);
            LobbyManager.Instance.OpenLobby();
            Debug.Log("Joined lobby successfully.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to join lobby: {ex.Message}");
        }
    }
}
