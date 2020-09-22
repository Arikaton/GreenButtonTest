using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public static class Utils
{
    public static Vector2 ConvertToV2(Vector3 vector)
    {
        return new Vector2(vector.x, vector.z);
    }

    public static int GetLivesPlayerCount()
    {
        int livesPlayerCount = 0;
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            if (!player.Value.CustomProperties.ContainsKey("PlayerDied"))
            {
                livesPlayerCount++;
            }
        }
        return livesPlayerCount;
    }
}
