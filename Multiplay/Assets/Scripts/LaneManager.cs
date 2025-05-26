using Mali.Utils;
using UnityEngine;
using UnityEngine.Splines;

public class LaneManager : Singleton<LaneManager>
{
    public LaneController[] lanes;
    

    private LaneController selectedLane;
    public LaneController SelectedLine => selectedLane;

    public void Start()
    {
        lanes[0].OnRaycastHit();
    }

    public LaneController GetLane(int index)
    {
        if (index >= 0 && index < lanes.Length)
            return lanes[index].GetComponent<LaneController>();

        return null;
    }

    public void SelecteLane(LaneController newLane)
    {
        selectedLane?.DisableOutline();
        selectedLane = newLane;
    }
}

