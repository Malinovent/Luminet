using System;
using Unity.Netcode;
using UnityEngine;

public class BehaviourCapturePoint : NetworkBehaviour, ICapturePoint
{
    public Action OnStartCapture;
    public Action OnEndCapture;

    private bool isCapturing = false;
    
    public bool IsCapturing => isCapturing;
    
    private ulong clientID;

    private LightZone currentZone;

    public void Initialize(ulong clientID)
    {
        this.clientID = clientID;
    }
   
    void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;        
        LightZone zone = other.GetComponent<LightZone>();

        if (zone && zone.GetControllingClientId() != OwnerClientId && currentZone != zone)
        {
            currentZone = zone;
            zone.onPointCaptured += UpdateCaptureState;            
            StartCapture();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsServer) return;

        LightZone zone = other.GetComponent<LightZone>();
        if (zone && zone == currentZone)
        {
            StopCapture();
        }
    }

    private void UpdateCaptureState()
    {        
        if (currentZone.GetControllingClientId() == clientID)
        {
            StopCapture();
        }
    }

    public void StartCapture()
    {
        isCapturing = true;
        OnStartCapture?.Invoke();
    }

    public void StopCapture()
    {
        if(currentZone != null)
        {
            currentZone.onPointCaptured -= UpdateCaptureState;            
            currentZone = null;
        }
        
        isCapturing = false;
        OnEndCapture?.Invoke();
    }   

}

public interface ICapturePoint
{
    public void StartCapture();
    public void StopCapture();
    
}