using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Splines;

public class LaneController : MonoBehaviour
{
    [SerializeField] SplineContainer lane;
    public List<ShipBase> shipsOnLane = new List<ShipBase>();

    public SplineContainer Lane => lane;

    public void AddShip(ShipBase ship) { shipsOnLane.Add(ship); }
    public void RemoveShip(ShipBase ship) { shipsOnLane.Remove(ship); }

    public Vector3 GetFormationOffset(ShipBase ship, float baseSpacing = 1f, float maxFormationRangeT = 0.04f, float maxOffset = 2f)
    {
        // List of same-owner ships within maxFormationRangeT
        List<ShipBase> group = new List<ShipBase>();

        foreach (ShipBase s in shipsOnLane)
        {
            if (s == ship) continue;
            if (s.GetOwnerId() == ship.GetOwnerId())
            {
                if (Mathf.Abs(s.GetT() - ship.GetT()) < maxFormationRangeT)
                    group.Add(s);
            }
        }

        // Add self to group, then sort by t
        group.Add(ship);
        group.Sort((a, b) => a.GetT().CompareTo(b.GetT()));

        int slotIndex = group.IndexOf(ship);

        // Only apply offset if there are multiple ships in the group
        if (group.Count < 2)
            return Vector3.zero;

        // Only apply offset for "local player"
        //if (ship.GetOwnerId() != NetworkManager.Singleton.LocalClientId)
            //return Vector3.zero;

        // Zig-zag: alternate left/right
        float offsetAmount = ((slotIndex % 2 == 0) ? 1 : -1) * ((slotIndex + 1) / 2) * baseSpacing;
        offsetAmount = Mathf.Clamp(offsetAmount, -maxOffset, maxOffset);

        float t = ship.GetT();
        Vector3 tangent = ((Vector3)Lane.EvaluateTangent(t)).normalized;
        Vector3 up = Vector3.up;
        Vector3 right = Vector3.Cross(up, tangent);

        return right * offsetAmount;
    }

    public List<ShipBase> GetSortedShips()
    {
        shipsOnLane.Sort((a, b) => a.GetT().CompareTo(b.GetT()));
        return shipsOnLane;
    }

    public ShipBase GetShipAhead(ShipBase self, bool onlyEnemies = false)
    {
        GetSortedShips();
        float myT = self.GetT();
        foreach (var ship in shipsOnLane)
        {
            if (ship == self) continue;
            if (ship.GetT() > myT)
            {
                if (!onlyEnemies || ship.GetOwnerId() != self.GetOwnerId())
                    return ship;
            }
        }
        return null;
    }

    // Helper for all ships within a certain t-distance
    public List<ShipBase> GetEnemiesInRange(ShipBase requester, float rangeT)
    {
        var enemies = new List<ShipBase>();
        foreach (var ship in shipsOnLane)
        {
            if (ship == requester) continue;
            if (ship.GetOwnerId() != requester.GetOwnerId())
            {
                float dt = Mathf.Abs(ship.GetT() - requester.GetT());
                if (dt < rangeT)
                    enemies.Add(ship);
            }
        }
        return enemies;
    }

    
}
