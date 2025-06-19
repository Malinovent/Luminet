using Mono.Cecil.Cil;
using System.Collections;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyInputPassword : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private GameObject failFeedback;

    private string code = string.Empty;

    public void OpenContainer()
    {
        failFeedback.SetActive(false);
        container.SetActive(true);
    }

    public void CloseContainer()
    {
        container.SetActive(false);
    }

    public void InputPassword(string input)
    {
        code = input.ToUpper().Trim();
    }

    public void SubmitPassword()
    {
        if (string.IsNullOrEmpty(code))
        {
            Debug.LogWarning("Lobby code cannot be empty.");
            return;
        }

        TryJoinByCode(code);
    }

    private async void TryJoinByCode(string lobbyCode)
    {
        int nextPlayerNumber = 1;
        string displayName = $"Player {nextPlayerNumber}";

        try
        {
            await LobbyManager.Instance.JoinLobbyByCodeAsync(lobbyCode, displayName);
            //LobbyManager.Instance.OpenLobby();
            Debug.Log("Joined lobby successfully.");
            CloseContainer();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to join lobby with code '{lobbyCode}': {ex.Message}");
            failFeedback.SetActive(true);
        }
    }

}
