using System;
using UnityEngine;

public class BehaviourCapturePoint : MonoBehaviour, ICapturePoint
{
    public Action OnStartCapture;
    public Action OnEndCapture;

    private bool isCapturing = false;
    
    public bool IsCapturing => isCapturing;

    public void StartCapture()
    {
        isCapturing = true;
        OnStartCapture?.Invoke();
    }

    public void StopCapture()
    {
        isCapturing = false;
        OnEndCapture?.Invoke();
    }   

}

public interface  ICapturePoint
{
    public void StartCapture();
    public void StopCapture();
    
}