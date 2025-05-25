using UnityEngine;
using UnityEngine.Splines;

public class ShipHeavyFighter : ShipBase
{
    [SerializeField] BehaviourCapturePoint capturePointBehaviour;

    private HeavyFighterStates currentState = HeavyFighterStates.Move;

    public override void Initialize(SplineContainer assignedLane, int moveDirection, ulong ownerID)
    {
        base.Initialize(assignedLane, moveDirection, ownerID);
        capturePointBehaviour.Initialize(ownerID);
    }

    /*public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        capturePointBehaviour.Initialize(ownerId.Value);
    }*/


    private void OnEnable()
    {
        capturePointBehaviour.OnEndCapture += OnEndCapture;
        capturePointBehaviour.OnStartCapture += OnStartCapture;
    }

    private void OnDisable()
    {
        capturePointBehaviour.OnEndCapture -= OnEndCapture;
        capturePointBehaviour.OnStartCapture -= OnStartCapture;
    }

    private void Update()
    {
        if (!IsServer) return;

        switch(currentState)
        {
            case HeavyFighterStates.Move:
                MoveAlongSpline();
                break;
            case HeavyFighterStates.Attack:
                AttackBehaviour();
                break;
        }
    }

    private void OnEndCapture()
    {
        Debug.Log("OnEndCapture called");
        if (currentState != HeavyFighterStates.Attack)
        {
            Debug.Log("OnEndCapture called and currentState is not Attack");
            SetState(HeavyFighterStates.Move);
        }
    }

    private void OnStartCapture()
    {
        if(currentState != HeavyFighterStates.Attack)
        {
            SetState(HeavyFighterStates.Capture);
        }        
    }

    private void AttackBehaviour()
    {
        //Do damage periodically to priority target
    }

    private void SetState(HeavyFighterStates newState)
    {
        currentState = newState;
    }
    
    private enum HeavyFighterStates
    {
        Move,
        Capture,
        Attack
    }
}
