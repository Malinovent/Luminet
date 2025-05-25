using Mali.Utils;
using UnityEngine;
using UnityEngine.Splines;

public class LaneManager : Singleton<LaneManager>
{
    public LaneController[] lanes; 

    public LaneController GetLane(int index)
    {
        if (index >= 0 && index < lanes.Length)
            return lanes[index].GetComponent<LaneController>();

        return null;
    }
}

