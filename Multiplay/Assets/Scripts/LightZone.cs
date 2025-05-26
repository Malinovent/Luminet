using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System;
using UnityEditor;

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
    private NetworkVariable<bool> myShipPresent = new NetworkVariable<bool>(false);
    private NetworkList<ulong> playersWithShips = new NetworkList<ulong>();

    NetworkVariable<float> timerProgress = new NetworkVariable<float>(0f);


    public Action onPointCaptured;

    public ulong GetControllingClientId()
    {
        return controllingClientId.Value;
    }

    private void Start()
    {
        controllingClientId.OnValueChanged += (oldVal, newVal) => UpdateVisual();
        myShipPresent.OnValueChanged += (oldVal, newVal) => UpdateVisual();
        playersWithShips.OnListChanged += (change) => UpdateVisual();
        timerProgress.OnValueChanged += (oldVal, newVal) => UpdateTimerVisual();

        if(startingClientId != NEUTRAL)
        {
            timerProgress.Value = 1.5f;
        }

        UpdateVisual();
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

            UpdateProgress();
            return;
        }

        if (playerWithMostShipsID != controllingClientId.Value)
        {
            counter += Time.deltaTime * numberOfShipsDifference;
            UpdateProgress();
            
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
                UpdatePlayersWithShips();
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

        //FindMyShip();
        UpdatePlayersWithShips();
    }

    private void UpdatePlayersWithShips()
    {
        playersWithShips.Clear();
        foreach (var ship in allShipsInZone)
        {
            ulong ownerId = ship.GetOwnerId();
            if (!playersWithShips.Contains(ownerId))
                playersWithShips.Add(ownerId);
        }
    }

    private void UpdateProgress()
    {
        if (controllingClientId.Value != NEUTRAL)
        {
            float normalizedProgress = Mathf.Clamp01(counter / maxCountInSeconds);
            timerProgress.Value = 1.5f - normalizedProgress * 1.5f;
        }
        else
        {
            float normalizedProgress = Mathf.Clamp01(counter / maxCountInSeconds);
            timerProgress.Value = normalizedProgress * 1.5f;
        }
    }

    private void UpdateTimerVisual()
    {
        zoneRenderer.GetComponent<Renderer>().material.SetFloat("_Progress", timerProgress.Value);
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

        bool isControlledByMe = NetworkManager.Singleton.LocalClientId == controllingClientId.Value;
        
        zoneRenderer.GetComponent<Renderer>().material.color = 
            controllingClientId.Value == NEUTRAL ? Color.white : PlayerColors.GetColorForClient(controllingClientId.Value);

        if(opaqueRenderer == null) return;

        ulong myClientId = NetworkManager.Singleton.LocalClientId;
        bool myShipPresent = playersWithShips.Contains(myClientId);

        if (isControlledByMe || myShipPresent)
        {
            opaqueRenderer.SetActive(false);
        }
        else
        {
            opaqueRenderer.SetActive(true);
            opaqueRenderer.GetComponent<Renderer>().material.color =
                controllingClientId.Value == NEUTRAL ? Color.white : PlayerColors.GetColorForClient(controllingClientId.Value);
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        controllingClientId.OnValueChanged -= (oldVal, newVal) => UpdateVisual();
        myShipPresent.OnValueChanged -= (oldVal, newVal) => UpdateVisual();
        playersWithShips.OnListChanged -= (change) => UpdateVisual();
        timerProgress.OnValueChanged -= (oldVal, newVal) => UpdateTimerVisual();
    }
}
