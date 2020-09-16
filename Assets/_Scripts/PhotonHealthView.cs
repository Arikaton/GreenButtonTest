using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Diagnostics;

public class PhotonHealthView : MonoBehaviourPunCallbacks, IPunObservable
{
    HealthProvider healthProvider;

    private void Start()
    {
        healthProvider = GetComponent<HealthProvider>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
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
