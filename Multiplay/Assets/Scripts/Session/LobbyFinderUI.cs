using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class LobbyFinderUI : MonoBehaviour
{
    [SerializeField] private GameObject lobbyContainerPrefab;
    [SerializeField] private Transform lobbiesParent;
    private List<LobbyContainerUI> lobbyContainers = new List<LobbyContainerUI>();

    private void Start()
    {
        _ = RefreshAsync();
    }

    public async void Refresh()
    {
        await RefreshAsync();
    }

    public async System.Threading.Tasks.Task RefreshAsync()
    {
        // Fetch lobbies asynchronously, don't block main thread!
        Debug.Log("Refreshing lobbies...");
        List<Lobby> lobbies = await LobbyManager.Instance.GetPublicLobbiesAsync();

        // Clean up old containers
        foreach (LobbyContainerUI container in lobbyContainers)
        {
            Destroy(container.gameObject);
        }
        lobbyContainers.Clear();

        // Create new UI for each lobby
        foreach (Lobby lobby in lobbies)
        {
            LobbyContainerUI container = Instantiate(lobbyContainerPrefab, transform).GetComponent<LobbyContainerUI>();
            container.transform.SetParent(lobbiesParent, false);
            container.SetLobbyData(lobby);
            lobbyContainers.Add(container);
            Debug.Log($"Lobby found: {lobby.Name} with {lobby.Players.Count}/{lobby.MaxPlayers} players.");
            Debug.Log(
               $"Lobby: {lobby.Name}, LobbyCode: {lobby.LobbyCode}, " +
               $"Id: {lobby.Id}, IsPrivate: {lobby.IsPrivate}, " +
               $"IsLocked: {lobby.IsLocked}, Players: {lobby.Players.Count}, " +
               $"Created: {lobby.Created}, LastUpdated: {lobby.LastUpdated}"
           );
        }
    }
}
