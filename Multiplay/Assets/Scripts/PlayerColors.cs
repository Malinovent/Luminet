using UnityEngine;

public static class PlayerColors
{
    public static readonly Color[] Colors = { Color.blue, Color.red, Color.green, Color.yellow }; // 0 = Player 1, 1 = Player 2, 2 = Player 3, 3 = Player 4
    public static Color GetColorForClient(ulong clientId) => Colors[(int)(clientId % (ulong)Colors.Length)];

}
