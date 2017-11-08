using UnityEngine;

public class LayerMasks : ScriptableObject
{
    public static LayerMask onlyGround = 1 << 8;
    public static LayerMask onlyWalls = 1 << 9;
    public static LayerMask onlyDoors = 1 << 10;
    public static LayerMask onlyKeys = 1 << 11;
    public static LayerMask onlyHunters = 1 << 12;
}