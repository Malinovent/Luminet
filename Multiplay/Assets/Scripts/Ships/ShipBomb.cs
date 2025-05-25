using UnityEngine;

public class ShipBomb : ShipBase
{
    
    protected override void OnPathEndReached()
    {
        Debug.Log("Game over for other player");
    }
}
