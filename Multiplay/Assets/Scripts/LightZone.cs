using UnityEngine;
using Unity.Netcode;
using System.Threading;
using System.Collections.Generic;

public class LightZone : NetworkBehaviour
{
    private const ulong NEUTRAL = 99;

    public ulong startingClientId = 0; // The client ID that starts the game with this zone

    private NetworkVariable<ulong> controllingClientId = new NetworkVariable<ulong>(0);
    [SerializeField] private GameObject zoneRenderer;
    [SerializeField] private int maxCountInSeconds = 10;

    private Dictionary<ulong, int> playerShipCount = new Dictionary<ulong, int>();
    private List<ShipBase> allShipsInZone = new List<ShipBase>();

    private ulong playerWithMostShipsID = NEUTRAL;
    private int numberOfShipsDifference = 0;
    private float counter = 0;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"OnTriggerEnter called with {other.name}");
        if (!IsServer) return;
        Debug.Log($"OnTriggerEnter called with {other.name} on server");
        var ship = other.GetComponent<ShipBase>();
        if (ship != null && !allShipsInZone.Contains(ship))
        {
            Debug.Log($"Ship detected: {ship.name}");
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

        if (playerWithMostShipsID == NEUTRAL && counter > 0)
        {
            counter -= Time.deltaTime;
            if(counter < 0)
            {
                counter = 0;
            }
            return;
        }

        if (playerWithMostShipsID != controllingClientId.Value)
        {
            counter += Time.deltaTime * numberOfShipsDifference;
            
            if (counter >= maxCountInSeconds)
            {
                //Set to neutral if no one is in the zone
                if (controllingClientId.Value != NEUTRAL)
                {
                    controllingClientId.Value = NEUTRAL;                   
                }
                else
                {
                    controllingClientId.Value = playerWithMostShipsID;
                }

                counter = 0;
            }

        }
    }

    private void CalculatePlayerWithMostShips()
    {
        if(allShipsInZone.Count == 0)
        {
            controllingClientId.Value = NEUTRAL;
            playerWithMostShipsID = NEUTRAL;
            numberOfShipsDifference = 0;
            return;
        }

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
            Debug.Log($"LightZone started with controlling client ID: {startingClientId}");
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
        Debug.Log($"DebugChangeOwner called. Current controlling client ID: {controllingClientId.Value}");
        controllingClientId.Value++;
        if (controllingClientId.Value > 1)
        {
            controllingClientId.Value = 0;
        }
    }
}
