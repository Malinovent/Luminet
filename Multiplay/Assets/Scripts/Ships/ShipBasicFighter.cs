using UnityEngine;
using UnityEngine.Splines;

public class ShipBasicFighter : ShipBase
{
    [SerializeField] BehaviourCapturePoint capturePointBehaviour;
    [SerializeField] BehaviourAttackPlayer playerAttackBehaviour;
    IAttackBehaviour attackBehaviour;

    private BasicFighterState currentState = BasicFighterState.Move;

    public override void Initialize(LaneController laneController, int moveDirection, ulong ownerID)
    {
        base.Initialize(laneController, moveDirection, ownerID);
        capturePointBehaviour.Initialize(ownerID);
        attackBehaviour.Initialize(laneController);
        playerAttackBehaviour.Initialize(laneController, direction);
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
            case BasicFighterState.Move:
                MoveAlongSpline();
                break;
            case BasicFighterState.Attack:
                AttackBehaviour();
                break;
            case BasicFighterState.AttackPlayer:
                AttackPlayerBehaviour();
                break;
        }
    }

    private void CheckForEnemy()
    {
        if (attackBehaviour.HasEnemyInRange())
        {
            SetState(BasicFighterState.Attack);
        }
        else if(capturePointBehaviour.IsCapturing)
        {
            SetState(BasicFighterState.Capture);
        }
        else if (currentState == BasicFighterState.Attack && !attackBehaviour.HasEnemyInRange())
        {
            SetState(BasicFighterState.Move);
        }
    }

    private void OnEndCapture()
    {
        //Debug.Log("OnEndCapture called");
        if (currentState != BasicFighterState.Attack)
        {
            //Debug.Log("OnEndCapture called and currentState is not Attack");
            SetState(BasicFighterState.Move);
        }
    }

    private void OnStartCapture()
    {
        if(currentState != BasicFighterState.Attack)
        {
            SetState(BasicFighterState.Capture);
        }        
    }

    private void AttackBehaviour()
    {
        attackBehaviour.PerformAttack();        
    }

    private void AttackPlayerBehaviour()
    {
        playerAttackBehaviour.PerformAttack();
    }

    private void SetState(BasicFighterState newState)
    {
        currentState = newState;
    }

    protected override void OnPathEndReached()
    {
        SetState(BasicFighterState.AttackPlayer);
    }

    private enum BasicFighterState
    {
        Move,
        Capture,
        Attack,
        AttackPlayer
    }
}
