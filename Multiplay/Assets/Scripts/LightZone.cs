using UnityEngine;
using Unity.Netcode;
using System.Threading;
using System.Collections.Generic;

public class LightZone : NetworkBehaviour
{
    public ulong startingClientId = 0; // The client ID that starts the game with this zone

    private NetworkVariable<ulong> controllingClientId = new NetworkVariable<ulong>(0);
    [SerializeField] private GameObject zoneRenderer;
    [SerializeField] private int maxCountInSeconds = 10;

    private Dictionary<ulong, int> playerShipCount = new Dictionary<ulong, int>();
    private List<ShipBase> allShipsInZone = new List<ShipBase>();

    private ulong playerWithMostShipsID = 0;
    private int numberOfShipsDifference = 0;
    private float counter = 0;

    private void Awake()
    {
        playerShipCount[0] = 0;
        playerShipCount[1] = 0;
        playerShipCount[2] = 0;
        playerShipCount[3] = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        var ship = other.GetComponent<ShipBase>();
        if (ship != null)
        {
            ulong shipOwner = ship.GetOwnerId();
            if (!playerShipCount.ContainsKey(shipOwner))
            {
                playerShipCount[shipOwner] = 0;
            }

            playerShipCount[shipOwner]++;
            allShipsInZone.Add(ship);

            CalculatePlayerWithMostShips();
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (!IsServer) return;

        var ship = collision.GetComponent<ShipBase>();
        if (ship != null)
        {
            ulong shipOwner = ship.GetOwnerId();
            playerShipCount[shipOwner]--;
            allShipsInZone.Remove(ship);
            CalculatePlayerWithMostShips();
        }
    }

    private void Update()
    {
        if(!IsServer) return;

        if (playerWithMostShipsID != controllingClientId.Value)
        {
            counter += Time.deltaTime * numberOfShipsDifference;
            //Set to neutral if no one is in the zone
            if (counter >= maxCountInSeconds && controllingClientId.Value != 99)
            {
                controllingClientId.Value = 99;
                counter = 0;
            }
            else
            {
                controllingClientId.Value = playerWithMostShipsID;
                counter = 0;
            }
        }
    }

    private void CalculatePlayerWithMostShips()
    {
        ulong maxPlayer = 0;
        int maxCount = 0;
        int secondMax = 0;

        foreach (var kvp in playerShipCount)
        {
            if (kvp.Value > maxCount)
            {
                secondMax = maxCount;
                maxCount = kvp.Value;
                maxPlayer = kvp.Key;
            }
            else if (kvp.Value > secondMax)
            {
                secondMax = kvp.Value;
            }
        }

        // Only assign control if lead is clear
        if (maxCount > 0 && maxCount > secondMax)
        {
            playerWithMostShipsID = maxPlayer;
            numberOfShipsDifference = maxCount - secondMax;
        }
    }

    private void Start()
    {
        controllingClientId.OnValueChanged += (oldVal, newVal) => UpdateVisual();
        UpdateVisual();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            controllingClientId.Value = startingClientId;
        }

        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (zoneRenderer == null) return;
        
        bool shouldShow = NetworkManager.Singleton.LocalClientId == controllingClientId.Value;
        zoneRenderer.SetActive(shouldShow);
    }

    public void DebugChangeOwner()
    {
        if (!IsServer) return;

        controllingClientId.Value++;
        if (controllingClientId.Value > 2)
        {
            controllingClientId.Value = 0;
        }
    }
}
