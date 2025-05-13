using UnityEngine;

public static class GeneralUtilities 
{
    public static Vector3 Flatten(this Vector3 origin)
    {
        return new Vector3(origin.x, 0f, origin.z);
    }
}
