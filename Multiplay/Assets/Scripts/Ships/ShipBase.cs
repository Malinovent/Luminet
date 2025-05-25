using Unity.Netcode;
using UnityEngine;
using UnityEngine.Splines;

public abstract class ShipBase : NetworkBehaviour
{
    [SerializeField] float speed = 0.2f; 
    [SerializeField] float formationOffset = 1.0f; 
    [SerializeField] PlayerVisuals playerVisuals;
    [SerializeField] GameObject lightMask;
    [SerializeField] Health3D health;
    [SerializeField] GameObject explosionPrefab;
    
    private LaneController laneController;
    private SplineContainer lane;
    private int direction = 1;
    private float t;

    protected NetworkVariable<ulong> ownerId = new NetworkVariable<ulong>();

    public float GetT() => t;

    public virtual void Initialize(LaneController laneController, int moveDirection, ulong ownerID)
    {
        this.laneController = laneController;
        lane = laneController.Lane;
        direction = moveDirection;
        t = direction == 1 ? 0f : 1f;
        transform.position = lane.EvaluatePosition(t);


        if (IsServer)
            ownerId.Value = ownerID;

        laneController.AddShip(this);
        SetVisuals();
        // playerVisuals.SetClientID(ownerID);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
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

    public void TakeDamage(int amount)
    {
        health.TakeDamage(amount);
    }

    protected void MoveAlongSpline()
    {
        if (lane == null)
            return;

        t += direction * speed * Time.deltaTime;
        t = Mathf.Clamp01(t);

        Vector3 basePos = lane.EvaluatePosition(t);
        
        Vector3 offset = laneController.GetFormationOffset(this, formationOffset); 

        transform.position = Vector3.Lerp(transform.position, basePos + offset, Time.deltaTime * 10f);

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

    public void Die()
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        // Cleanly remove from lane controller if present
        if (laneController != null)
            laneController.RemoveShip(this);
    }

}
