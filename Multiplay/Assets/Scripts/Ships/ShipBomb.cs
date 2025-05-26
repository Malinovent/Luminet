using UnityEngine;

public class ShipBomb : ShipBase
{
    [SerializeField] BehaviourCapturePoint capturePointBehaviour;

    public override void Initialize(LaneController laneController, int moveDirection, ulong ownerID)
    {
        base.Initialize(laneController, moveDirection, ownerID);
        capturePointBehaviour.Initialize(ownerID);
    }

    private void Update()
    {
        if (!IsServer) return;

        MoveAlongSpline();        
    }

}
