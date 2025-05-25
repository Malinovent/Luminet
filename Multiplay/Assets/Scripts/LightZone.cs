using UnityEngine;
using Unity.Netcode;
using System.Threading;
using System.Collections.Generic;
using System;

public class LightZone : NetworkBehaviour
{
    private const ulong NEUTRAL = 99;

    public ulong startingClientId = 0; // The client ID that starts the game with this zone

    private NetworkVariable<ulong> controllingClientId = new NetworkVariable<ulong>(0);
    [SerializeField] private GameObject zoneRenderer;
    [SerializeField] private GameObject opaqueRenderer;
    [SerializeField] private int maxCountInSeconds = 10;

    private Dictionary<ulong, int> playerShipCount = new Dictionary<ulong, int>();
    private List<ShipBase> allShipsInZone = new List<ShipBase>();

    private ulong playerWithMostShipsID = NEUTRAL;
    private int numberOfShipsDifference = 0;
    private float counter = 0;
    private bool enemyIsInZone = false;

    public Action onPointCaptured;

    public ulong GetControllingClientId()
    {
        return controllingClientId.Value;
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (!IsServer) return;
        
        ShipBase ship = other.GetComponent<ShipBase>();
        if (ship != null && !allShipsInZone.Contains(ship))
        {
            ulong shipOwner = ship.GetOwnerId();
            if (!playerShipCount.ContainsKey(shipOwner))
            {
                playerShipCount[shipOwner] = 0;
            }            

            playerShipCount[shipOwner]++;
            allShipsInZone.Add(ship);

            CalculatePlayerWithMostShips();
            IsEnemyInZone();
            UpdateVisual();
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (!IsServer) return;

        ShipBase ship = collision.GetComponent<ShipBase>();
        if (ship != null)
        {
            ulong shipOwner = ship.GetOwnerId();
            playerShipCount[shipOwner]--;
            allShipsInZone.Remove(ship);
            CalculatePlayerWithMostShips();
            IsEnemyInZone();
            UpdateVisual();
        }
    }

    private void Update()
    {
        if(!IsServer) return;

        if (playerWithMostShipsID == NEUTRAL && controllingClientId.Value == NEUTRAL && counter > 0)
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

                onPointCaptured?.Invoke();
                counter = 0;
                IsEnemyInZone();
            }

        }
    }

    private void CalculatePlayerWithMostShips()
    {
        if(allShipsInZone.Count == 0)
        {
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

    private void IsEnemyInZone()
    {
        if (controllingClientId.Value == NEUTRAL || allShipsInZone.Count > 0)
        {
            enemyIsInZone = true;
            return;
        }
       
        foreach (ShipBase ship in allShipsInZone)
        {
            if (ship.GetOwnerId() != controllingClientId.Value)
            {
                enemyIsInZone = true;
                return;
            }
        }

        enemyIsInZone = false;
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
        //zoneRenderer.SetActive(shouldShow);
        zoneRenderer.GetComponent<Renderer>().material.color = controllingClientId.Value == NEUTRAL ? Color.white : PlayerColors.GetColorForClient(controllingClientId.Value);

        if(opaqueRenderer == null) return;

        if (enemyIsInZone)
        {
            opaqueRenderer.SetActive(false);
            return;
        }

        opaqueRenderer.SetActive(!shouldShow);
        opaqueRenderer.GetComponent<Renderer>().material.color = controllingClientId.Value == NEUTRAL ? Color.white : PlayerColors.GetColorForClient(controllingClientId.Value);
    }

    public void DebugChangeOwner()
    {
        if (!IsServer) return;
        
        controllingClientId.Value++;
        if (controllingClientId.Value > 1)
        {
            controllingClientId.Value = 0;
        }
    }
}
