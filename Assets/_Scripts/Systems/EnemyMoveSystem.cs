﻿using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(EnemyMoveSystem))]
public sealed class EnemyMoveSystem : UpdateSystem {
    public GameConfig gameConfig;
    Filter enemyFilter;

    public override void OnAwake() {
        enemyFilter = World.Filter.With<EnemyComponent>().With<MoveViewComponent>();
    }

    public override void OnUpdate(float deltaTime) {
        var enemyBag = enemyFilter.Select<EnemyComponent>();
        var moveBag = enemyFilter.Select<MoveViewComponent>();

        for (int i = 0, lenght = enemyFilter.Length; i < lenght; i++)
        {
            ref var enemy = ref enemyBag.GetComponent(i);
            ref var move = ref moveBag.GetComponent(i);

            if (enemy.aim != null)
            {
                move.agent.SetDestination(enemy.aim.GetComponent<MoveViewComponent>().transform.position);
            } else
            {
                if (Vector2.Distance(Utils.ConvertToV2(move.transform.position), Utils.ConvertToV2(move.agent.destination)) < 0.5f)
                {
                    move.agent.destination = GetRandomVector3();
                }
            }
        }
    }
    
    Vector3 GetRandomVector3()
    {
        return new Vector3(
            Random.Range(-gameConfig.levelScale.x * 5, gameConfig.levelScale.x * 5),
            0,
            Random.Range(-gameConfig.levelScale.y * 5, gameConfig.levelScale.y * 5));
    }
}