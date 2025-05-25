using System.Globalization;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.Splines;

public class PlayerShipSpawner : NetworkBehaviour
{

    [SerializeField] private GameObject shipPrefab; // Prefab with NetworkObject, NetworkTransform, ShipMovement
    [SerializeField] private PlayerVisuals playerVisuals;

    private ulong myClientId;
    private int myDirection; // 1 for A->B, -1 for B->A

    private void Start()
    {
        myClientId = NetworkManager.Singleton.LocalClientId;        
        myDirection = myClientId == 0 ? 1 : -1;
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            RequestSpawnShipServerRpc(0, myDirection);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            RequestSpawnShipServerRpc(1, myDirection);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            RequestSpawnShipServerRpc(2, myDirection);
    }

    public override void OnNetworkSpawn()
    {
        //SetColorServerRpc();
        base.OnNetworkSpawn();
        playerVisuals.SetClientID(OwnerClientId);
    }

    /*[ServerRpc]
    void SetColorServerRpc(ServerRpcParams rpcParams = default)
    {
        playerVisuals.SetClientID(rpcParams.Receive.SenderClientId);
    }*/


    [ServerRpc]
    void RequestSpawnShipServerRpc(int laneIndex, int direction, ServerRpcParams rpcParams = default)
    {
        SplineContainer lane = LaneManager.Instance.GetLane(laneIndex);
        if (lane == null) return;

        ulong requestingClientId = rpcParams.Receive.SenderClientId;

        Vector3 spawnPos = direction == 1  ? lane.EvaluatePosition(0f) : lane.EvaluatePosition(1f);

        GameObject ship = Instantiate(shipPrefab, spawnPos, Quaternion.identity);
        NetworkObject netObj = ship.GetComponent<NetworkObject>();
        netObj.Spawn();

        // Initialize movement on the server
        ShipBase move = ship.GetComponent<ShipBase>();
        move.Initialize(lane, direction, requestingClientId);
    }
}
