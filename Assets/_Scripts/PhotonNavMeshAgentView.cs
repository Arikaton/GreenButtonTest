using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PhotonNavMeshAgentView : MonoBehaviour, IPunObservable
{
    NavMeshAgent agent;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
        if (stream.IsWriting)
        {
            stream.SendNext(agent.destination.x);
            stream.SendNext(agent.destination.y);
            stream.SendNext(agent.destination.z);
        } else
        {
            float newX = (float)stream.PeekNext();
            float newY = (float)stream.PeekNext();
            float newZ = (float)stream.PeekNext();
            agent.destination = new Vector3(newX, newY, newZ);
        }
    }
}
