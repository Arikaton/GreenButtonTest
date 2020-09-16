using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Photon.Pun;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(PlayerSystem))]
public sealed class PlayerSystem : UpdateSystem {
    Filter filter;
    Camera camera;

    public override void OnAwake() {
        filter = World.Filter.With<PlayerComponent>();
        camera = Camera.main;
        //ref var photonView = ref filter.Select<PhotonViewComponent>().GetComponent(0).photonView;
        Debug.LogFormat("<color=yellow>{0}</color>: Photon is MasterClient - {1}", this.ToString(), PhotonNetwork.IsMasterClient);
        //Debug.LogFormat("<color=yellow>{0}</color>: Photon is Mine - {1}", this.ToString(), photonView.IsMine);
    }

    public override void OnUpdate(float deltaTime) {
        foreach(var entity in filter)
        {
            ref var photonView = ref entity.GetComponent<PhotonViewComponent>().photonView;
            if (!photonView.IsMine) continue;
            ref var playerComponent = ref entity.GetComponent<PlayerComponent>();
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
    }
}