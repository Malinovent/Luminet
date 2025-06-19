using TMPro;
using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using UnityEngine.UI;
using System.Threading.Tasks;

public class ActiveLobbyPlayer : MonoBehaviour
{
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private Button kickButton;

    private string playerId;
    private string lobbyId;
    private string hostId;

    public void SetPlayer(string name, string id, string lobbyId, string hostId)
    {
        this.playerId = id;
        this.lobbyId = lobbyId;
        this.hostId = hostId;
        UpdatePlayerName(name);
        UpdateKickButton();
    }

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

    private void UpdateKickButton()
    {
        if (kickButton == null) return;

        string localPlayerId = AuthenticationService.Instance.PlayerId;

        bool isLocalPlayerHost = localPlayerId == hostId;
        bool isTargetPlayerHost = playerId == hostId;

        // Show the kick button ONLY if:
        // - I'm the host
        // - I'm not trying to kick the host (including myself if I'm the host)
        kickButton.gameObject.SetActive(isLocalPlayerHost && !isTargetPlayerHost);
    }

    public async void KickPlayer()
    {
        if (string.IsNullOrEmpty(playerId) || string.IsNullOrEmpty(lobbyId))
        {
            Debug.LogWarning("Cannot kick player. Missing playerId or lobbyId.");
            Destroy(this.gameObject); // Optionally remove from UI
            return;
        }

        try
        {
            await LobbyService.Instance.RemovePlayerAsync(lobbyId, playerId);
            Debug.Log($"Kicked player {playerName.text} ({playerId}) from lobby.");
            Destroy(gameObject); // Optionally remove from UI
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogError($"Failed to kick player: {ex.Message}");
        }
    }
}
