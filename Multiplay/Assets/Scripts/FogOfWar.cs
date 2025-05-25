using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    LightZone[] lightZones;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lightZones = FindObjectsByType<LightZone>(FindObjectsSortMode.None);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            DebugRevealZones();
        }
    }

    private void DebugRevealZones()
    {
        foreach(LightZone zone in lightZones)
        {
            zone.DebugChangeOwner();
        }
    }
}
