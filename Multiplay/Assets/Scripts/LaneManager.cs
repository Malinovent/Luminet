using Mali.Utils;
using UnityEngine;
using UnityEngine.Splines;

public class LaneManager : Singleton<LaneManager>
{
    public LaneController[] lanes;
    

    private LaneController selectedLane;
    public LaneController SelectedLane => selectedLane;

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

    public int GetSelectedLaneIndex()
    {
        for (int i = 0; i < lanes.Length; i++)
        {
            if (lanes[i] == SelectedLane)
                return i;
        }
        return -1;
    }

    public void SelectLane(LaneController newLane)
    {
        selectedLane?.DisableOutline();
        selectedLane = newLane;
    }
}

