﻿using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(PlayerMoveSystem))]
public sealed class PlayerMoveSystem : UpdateSystem {
    Filter filter;
    Filter gameManagerFilter;

    public override void OnAwake() {
        filter = World.Filter.With<MoveViewComponent>().With<PlayerComponent>();
        gameManagerFilter = World.Filter.With<GameManagerComponent>();
    }

    public override void OnUpdate(float deltaTime) {
        ref var gameManagerComponent = ref gameManagerFilter.First().GetComponent<GameManagerComponent>();
        if (gameManagerComponent.gameState != GameState.Playing) return;

        foreach (var entity in filter)
        {
            ref var photonViewComponent = ref entity.GetComponent<PhotonViewComponent>();
            if (!photonViewComponent.photonView.IsMine) continue;

            ref var healthComponent = ref entity.GetComponent<HealthComponent>();
            if (healthComponent.health <= 0) continue;

            ref var playerComponent = ref entity.GetComponent<PlayerComponent>();
            if (playerComponent.destination == null) continue;

            ref var playerViewComponent = ref entity.GetComponent<MoveViewComponent>();
            
            playerViewComponent.agent.SetDestination((Vector3)playerComponent.destination);
        }
    }
}