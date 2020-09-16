using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Diagnostics;

public class PhotonHealthView : MonoBehaviourPunCallbacks, IPunObservable
{
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        var healthProvider = GetComponent<HealthProvider>();
        ref var healthData = ref healthProvider.GetData();
        if (stream.IsWriting)
        {
            stream.SendNext(healthData.health);
        } else
        {
            healthData.health = (int)stream.ReceiveNext();
        }
    }

}
