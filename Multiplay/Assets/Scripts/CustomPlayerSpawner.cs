using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;

public class CustomPlayerSpawner : MonoBehaviour
{
    [SerializeField] Transform[] spawnPoints;

    //private int nextSpawnIndex = 0;

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += SetupSpawnHandler;        
    }

    //Only becomes relevant when the server starts
    void SetupSpawnHandler()
    {
        // When a client tries to connect, this callback will decide if they're allowed to join.
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;

        // Once a client is approved and connected, this callback is triggered.
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    /// <summary>
    /// Called whenever a client tries to connect to the server.
    /// You can reject or approve the connection, and choose whether to spawn the player prefab.
    /// </summary>
    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        Debug.Log("ApprovalCheck called");
        response.Approved = true;
        response.CreatePlayerObject = true;

        // Figure out which spawn point this player should use
        int clientCount = NetworkManager.Singleton.ConnectedClients.Count;
        int spawnIndex = clientCount % spawnPoints.Length;

        if (spawnPoints.Length > 0)
        {
            response.Position = spawnPoints[spawnIndex].position;
            response.Rotation = spawnPoints[spawnIndex].rotation;
        }
    }

    /// <summary>
    /// Called after a client connects and their player prefab is created.
    /// Here we move their player to a unique spawn point.
    /// </summary>
    private void OnClientConnected(ulong clientId)
    {
        if (spawnPoints.Length <= 0) return;

        //Debug.Log("OnClientConnected called");

        NetworkClient client;

        // Try to get the client object, which contains the player's NetworkObject.
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out client))
        {
            // Get the player's root GameObject
            NetworkObject playerNetworkObject = client.PlayerObject;
            if (playerNetworkObject != null)
            {
                // Assign a spawn point based on their client ID (wraps around if more players than points)
                int spawnIndex = (int)clientId % spawnPoints.Length;                
                playerNetworkObject.transform.position = spawnPoints[spawnIndex].position;
                //Debug.Log($"Client {clientId} connected, spawning at index {spawnIndex}. Position {spawnPoints[spawnIndex].position}");
            }
        }
    }

}
