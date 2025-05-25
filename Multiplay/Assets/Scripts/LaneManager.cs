using Mali.Utils;
using UnityEngine;
using UnityEngine.Splines;

public class LaneManager : Singleton<LaneManager>
{
    public SplineContainer[] lanes; 

    public SplineContainer GetLane(int index)
    {
        if (index >= 0 && index < lanes.Length)
            return lanes[index];

        return null;
    }
}
