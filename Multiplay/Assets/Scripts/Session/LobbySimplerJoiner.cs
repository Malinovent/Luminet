using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Collections.Generic;
using System.Threading.Tasks;

public class LobbySimplerJoiner : MonoBehaviour
{
    [SerializeField] private GameObject container;
    private string lobbyCode = string.Empty;
    private Lobby currentLobby;

    public void SetLobbyCode(string code)
    {
        lobbyCode = code;
    }

    public async void TryJoinLobby()
    {
        try
        {
            await JoinLobbyByCode();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to join lobby: {ex.Message}");
        }

    }

    public async Task JoinLobbyByCode()
    {
        int nextPlayerNumber = Random.Range(1,9999);
        string displayName = $"Player {nextPlayerNumber}";

        try
        {
            // Step 1: Join the Lobby using the code
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

            // Step 2: Retrieve the Relay Join Code from lobby data
            if (!currentLobby.Data.TryGetValue("RelayJoinCode", out var relayCodeObj))
            {
                Debug.LogError("RelayJoinCode not found in lobby data.");
                return;
            }

            string relayJoinCode = relayCodeObj.Value;
            Debug.Log($"Joining Relay allocation with code: {relayJoinCode}");
            // Step 3: Join the Relay allocation
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);

            // Step 4: Setup Netcode transport with Relay
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData,
                false
            );

            // Step 5: Start the client
            NetworkManager.Singleton.StartClient();
            Debug.Log("Successfully joined lobby and started client.");
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogError($"Failed to join lobby: {ex.Message}");
            return;
        }

        LobbyManager.Instance.OpenLobby(currentLobby);
        container.SetActive(false);
    }
}
