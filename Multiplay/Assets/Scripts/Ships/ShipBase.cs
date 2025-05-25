using Unity.Netcode;
using UnityEngine;
using UnityEngine.Splines;

public abstract class ShipBase : NetworkBehaviour
{
    [SerializeField] float speed = 0.2f; // tweak for game feel
    [SerializeField] PlayerVisuals playerVisuals;
    [SerializeField] GameObject lightMask;

    private SplineContainer lane;
    private int direction = 1;
    private float t;

    private NetworkVariable<ulong> ownerId = new NetworkVariable<ulong>();

    public void InitializeLane(SplineContainer assignedLane, int moveDirection, ulong ownerID)
    {
        lane = assignedLane;
        direction = moveDirection;
        t = direction == 1 ? 0f : 1f;
        transform.position = lane.EvaluatePosition(t);


        if (IsServer)
            ownerId.Value = ownerID; 

       // playerVisuals.SetClientID(ownerID);
    }

    public override void OnNetworkSpawn()
    {
        SetVisuals();

        ownerId.OnValueChanged += (oldVal, newVal) => SetVisuals();
    }

    private void SetLightMask()
    {
        if (lightMask == null) return;
        
        bool isMyShip = NetworkManager.Singleton.LocalClientId == ownerId.Value;
        lightMask.SetActive(isMyShip);        
    }

    private void SetVisuals()
    {
        playerVisuals.SetClientID(ownerId.Value);
        SetLightMask();
    }

    /*void Update()
    {
        MoveAlongSpline();
    }*/

    protected void MoveAlongSpline()
    {
        if (lane == null)
            return;

        t += direction * speed * Time.deltaTime;
        t = Mathf.Clamp01(t);

        transform.position = lane.EvaluatePosition(t);

        if ((direction == 1 && t >= 1f) || (direction == -1 && t <= 0f))
            OnPathEndReached();
    }

    protected virtual void OnPathEndReached()
    {

    }

    public ulong GetOwnerId()
    {
        return ownerId.Value;
    }
}
