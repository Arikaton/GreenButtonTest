using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Photon.Pun;
using Morpeh.Globals;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun.UtilityScripts;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(PlayerSystem))]
public sealed class PlayerSystem : UpdateSystem, IInRoomCallbacks {
    public GlobalEvent leaveRoom;
    Filter filter;
    Camera camera;

    public override void OnAwake() {
        PhotonNetwork.AddCallbackTarget(this);
        filter = World.Filter.With<PlayerComponent>();
        camera = Camera.main;
    }

    public override void Dispose()
    {
        base.Dispose();
        PhotonNetwork.RemoveCallbackTarget(this);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnUpdate(float deltaTime) {
        foreach(var entity in filter)
        {
            ref var photonViewComponent = ref entity.GetComponent<PhotonViewComponent>();
            var photonView = photonViewComponent.photonView;
            ref var playerComponent = ref entity.GetComponent<PlayerComponent>();

            if (playerComponent.actorNumber == 0)
            {
                Debug.Log("ActorNumber: " + photonView.Owner.ActorNumber);
                playerComponent.actorNumber = photonView.Owner.ActorNumber;
            }
            playerComponent.meshRenderer.material = GetMaterialByPlayerNumber(playerComponent.actorNumber);

            if (!photonView.IsMine) continue;
            //ref var playerComponent = ref entity.GetComponent<PlayerComponent>();
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hitInfo;
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                bool hit = Physics.Raycast(ray, out hitInfo);
                if (hit)
                {
                    playerComponent.destination = hitInfo.point;
                }
            }
        }

        if (leaveRoom.IsPublished)
        {
            LeaveRoom();
        }
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
        throw new System.NotImplementedException();
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        throw new System.NotImplementedException();
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
        {
            LeaveRoom();
        }
    }

    private static void LeaveRoom()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        PhotonNetwork.LeaveRoom();
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        
    }

    Material GetMaterialByPlayerNumber(int playerNumber)
    {
        Material playerMaterial;
        switch (playerNumber)
        {
            case 1:
                playerMaterial = Resources.Load("RedPlayer") as Material;
                break;
            case 2:
                playerMaterial = Resources.Load("BluePlayer") as Material;
                break;
            default:
                playerMaterial = Resources.Load("RedPlayer") as Material;
                break;
        }
        return playerMaterial;
    }

    public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        throw new System.NotImplementedException();
    }
}