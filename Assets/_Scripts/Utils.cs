using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector2 ConvertToV2(Vector3 vector)
    {
        return new Vector2(vector.x, vector.z);
    }
}
