using UnityEngine;
using UnityEngine.Splines;

public class ShipHeavyFighter : ShipBase
{
    [SerializeField] BehaviourCapturePoint capturePointBehaviour;
    //[SerializeField] BehaviourAttackSingle attackBehaviour;
    IAttackBehaviour attackBehaviour;

    private HeavyFighterStates currentState = HeavyFighterStates.Move;

    public override void Initialize(LaneController laneController, int moveDirection, ulong ownerID)
    {
        base.Initialize(laneController, moveDirection, ownerID);
        capturePointBehaviour.Initialize(ownerID);
        attackBehaviour.Initialize(laneController);
    }

    private void Awake()
    {
        attackBehaviour = GetComponent<IAttackBehaviour>();
    }    

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

        CheckForEnemy();

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

    private void CheckForEnemy()
    {
        if (attackBehaviour.HasEnemyInRange())
        {
            SetState(HeavyFighterStates.Attack);
        }
        else if(capturePointBehaviour.IsCapturing)
        {
            SetState(HeavyFighterStates.Capture);
        }
        else if (currentState == HeavyFighterStates.Attack && !attackBehaviour.HasEnemyInRange())
        {
            SetState(HeavyFighterStates.Move);
        }
    }

    private void OnEndCapture()
    {
        //Debug.Log("OnEndCapture called");
        if (currentState != HeavyFighterStates.Attack)
        {
            //Debug.Log("OnEndCapture called and currentState is not Attack");
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
        attackBehaviour.PerformAttack();        
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
