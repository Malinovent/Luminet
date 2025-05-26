using Unity.Netcode;
using UnityEngine;

public class PlayerShipSpawner : NetworkBehaviour
{

    [SerializeField] private GameObject[] shipPrefabs; // Prefab with NetworkObject, NetworkTransform, ShipMovement
    [SerializeField] private PlayerVisuals playerVisuals;

    private PlayerScore playerScore;


    private void Start()
    {
        playerScore = GetComponent<PlayerScore>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            RequestSpawnShipServerRpc(0, 1, 0);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            RequestSpawnShipServerRpc(1, 3, 0);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            RequestSpawnShipServerRpc(2, 6, 0);
    }

    public override void OnNetworkSpawn()
    {
        //SetColorServerRpc();
        base.OnNetworkSpawn();
        playerVisuals.SetClientID(OwnerClientId);
        //myClientId = NetworkManager.Singleton.LocalClientId;

        if (IsOwner && UIManager.Instance != null)
        {
            UIManager.Instance.SetLocalPlayerSpawner(this);
        }
    }

    [ServerRpc]
    public void RequestSpawnShipServerRpc(int shipIndex, int shipCost, int laneIndex, ServerRpcParams rpcParams = default)
    {
        LaneController lane = LaneManager.Instance.GetLane(laneIndex);
        if (lane == null) return;

        if(playerScore.Resource.Value < shipCost)
        {
            return;
        }

        playerScore.Resource.Value -= shipCost;

        ulong requestingClientId = rpcParams.Receive.SenderClientId;

        int direction = requestingClientId == 0 ? 1 : -1;

        Vector3 spawnPos = direction == 1 ? lane.Lane.EvaluatePosition(0f) : lane.Lane.EvaluatePosition(1f);

        GameObject randomShip = shipPrefabs[shipIndex];

        GameObject ship = Instantiate(randomShip, spawnPos, Quaternion.identity);
        NetworkObject netObj = ship.GetComponent<NetworkObject>();
        netObj.Spawn();

        // Initialize movement on the server
        ShipBase move = ship.GetComponent<ShipBase>();
        move.Initialize(lane, direction, requestingClientId);
    }
}
