﻿using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(PlayerMoveSystem))]
public sealed class PlayerMoveSystem : UpdateSystem {
    Filter filter;

    public override void OnAwake() {
        filter = World.Filter.With<MoveViewComponent>().With<PlayerComponent>();
    }

    public override void OnUpdate(float deltaTime) {
        foreach (var entity in filter)
        {
            ref var photonViewComponent = ref entity.GetComponent<PhotonViewComponent>();
            if (!photonViewComponent.photonView.IsMine) continue;

            ref var playerComponent = ref entity.GetComponent<PlayerComponent>();
            ref var playerViewComponent = ref entity.GetComponent<MoveViewComponent>();

            playerViewComponent.agent.SetDestination(playerComponent.destination);
        }
    }
}