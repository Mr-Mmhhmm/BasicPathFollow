using UnityEngine;

public class LayerMasks : ScriptableObject
{
    public static LayerMask onlyWalls = 1 << 9;
    public static LayerMask onlyGround = 1 << 8;
}