using UnityEngine;

public class ShipHeavyFighter : ShipBase
{
    [SerializeField] BehaviourCapturePoint capturePoint;

    private HeavyFighterStates currentState = HeavyFighterStates.Move;

    private bool isCapturing = false;

    private void OnEnable()
    {
        capturePoint.OnEndCapture += OnEndCapture;
    }

    private void OnDisable()
    {
        capturePoint.OnEndCapture -= OnEndCapture;
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
        if (currentState != HeavyFighterStates.Attack)
        {
            SetState(HeavyFighterStates.Move);
        }
    }

   
    private void AttackBehaviour()
    {
        //Do damage periodically to priority target
    }

    private void SetState(HeavyFighterStates newState)
    {
        if(isCapturing && newState == HeavyFighterStates.Move)
        {
            currentState = HeavyFighterStates.Capture;
            return;
        }
        
        currentState = newState;
    }
    
    private enum HeavyFighterStates
    {
        Move,
        Capture,
        Attack
    }
}
