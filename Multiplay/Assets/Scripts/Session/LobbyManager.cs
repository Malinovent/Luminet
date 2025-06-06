using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using Mali.Utils;

public class LobbyManager : Singleton<LobbyManager>
{
    [SerializeField] GameObject lobbyUI;
    [SerializeField] GameObject lobbyFinderUI;
    [SerializeField] GameObject lobbyCreatorUI;

    private bool isPrivate = false;
    private string lobbyName = "Default Lobby";

    private Lobby currentLobby;
    private bool pollLobby = false;
    private float pollInterval = 2f; // seconds

    public async void StartPollingLobby(string lobbyId)
    {
        pollLobby = true;
        while (pollLobby)
        {
            await UpdateLobbyData(lobbyId);
            await Task.Delay((int)(pollInterval * 1000));
        }
    }

    public void StopPollingLobby()
    {
        pollLobby = false;
    }

    // Fetch the latest lobby data
    private async Task UpdateLobbyData(string lobbyId)
    {
        try
        {
            currentLobby = await LobbyService.Instance.GetLobbyAsync(lobbyId);
            
            lobbyUI.GetComponent<LobbyUI>().UpdatePlayers(currentLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Failed to fetch lobby: {e}");
        }
    }

    public void SetLobbyName(string lobbyName)
    {
        if (string.IsNullOrEmpty(lobbyName))
        {
            Debug.LogWarning("Lobby name cannot be empty. Using default name.");
            this.lobbyName = "Default Lobby";
        }
        else
        {
            this.lobbyName = lobbyName;
        }
    }

    public void SetPrivacy(bool isPrivate)
    {
        this.isPrivate = isPrivate;
        Debug.Log($"Lobby privacy set to: {(isPrivate ? "Private" : "Public")}");
    }

    public async Task<List<Lobby>> GetPublicLobbiesAsync()
    {
        var query = new QueryLobbiesOptions
        {
            Filters = new List<QueryFilter>
        {
            new QueryFilter(
                field: QueryFilter.FieldOptions.IsLocked,
                op: QueryFilter.OpOptions.EQ,
                value: "false")
        },
            Count = 25
        };

        var response = await LobbyService.Instance.QueryLobbiesAsync(query);
        return response.Results;
    }

    public async void StartHostSetup()
    {
        await SetupRelayHostAsync();

        // Start polling the lobby for updates
        StartPollingLobby(currentLobby.Id);
    }


    public async Task SetupRelayHostAsync()
    {
        // Create the Relay allocation
        var allocation = await RelayService.Instance.CreateAllocationAsync(1);

        // Get the join code (for sharing with clients)
        string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        // Create the lobby
        CreateLobbyOptions lobbyOptions = new CreateLobbyOptions
        {
            IsPrivate = isPrivate,
            Data = new Dictionary<string, DataObject>
            {
                { "RelayJoinCode", new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
            }
        };

        currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 2, lobbyOptions);

        // Set the allocation data directly
        UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        unityTransport.SetRelayServerData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData,
            allocation.ConnectionData,
            true
        );

        // Start the NGO host
        NetworkManager.Singleton.StartHost();

        lobbyCreatorUI.SetActive(false);
        lobbyFinderUI.SetActive(false);
        lobbyUI.SetActive(true);

        Debug.Log($"Lobby created and Relay Host started. Lobby code: {currentLobby.LobbyCode}, Relay code: {relayJoinCode}");
    }

    public void OpenLobby()
    {
        lobbyFinderUI.SetActive(false);
        lobbyUI.SetActive(true);

        StartPollingLobby(currentLobby.Id);
    }

    public async Task JoinLobbyByCodeAsync(string lobbyCode, string displayName)
    {
        try
        {
            // Join the lobby using the code
            var joinOptions = new JoinLobbyByCodeOptions
            {
                Player = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                {
                    { "DisplayName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, displayName) }
                }
                }
            };

            currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinOptions);

            // Get the relay join code from lobby data
            if (!currentLobby.Data.TryGetValue("RelayJoinCode", out var relayCodeObj))
            {
                Debug.LogError("RelayJoinCode not found in lobby data!");
                return;
            }
            string relayJoinCode = relayCodeObj.Value;

            // Join the Relay allocation
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);

            // Configure NGO transport
            UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            unityTransport.SetRelayServerData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData,
                false
            );

            // Start the NGO client
            NetworkManager.Singleton.StartClient();

            // Show UI
            lobbyFinderUI.SetActive(false);
            lobbyCreatorUI.SetActive(false);
            lobbyUI.SetActive(true);

            // Start polling for lobby updates
            StartPollingLobby(currentLobby.Id);

            Debug.Log("Joined lobby and started Relay client.");
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogError($"Failed to join lobby: {ex}");
        }
    }

    private void OnDestroy()
    {
        StopPollingLobby();        
    }
}
