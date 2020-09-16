using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using System.Collections.Generic;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(EnemySystem))]
public sealed class EnemySystem : UpdateSystem {
    public GameConfig gameConfig;
    Filter filter;
    Filter playerFilter;

    public override void OnAwake() {
        var commonFilter = World.Filter.With<MoveViewComponent>();
        filter = commonFilter.With<EnemyComponent>();
        playerFilter = commonFilter.With<PlayerComponent>();
    }

    public override void OnUpdate(float deltaTime) {
        var enemiesBag = filter.Select<EnemyComponent>();
        var moveBag = filter.Select<MoveViewComponent>();

        for (int i = 0, lenght = filter.Length; i < lenght; i++)
        {
            ref var enemy = ref enemiesBag.GetComponent(i);
            ref var enemyMove = ref moveBag.GetComponent(i);

            TryFindEnemy(ref enemy, ref enemyMove);
        }
    }

    private void TryFindEnemy(ref EnemyComponent enemy, ref MoveViewComponent enemyMove)
    {
        float minDistance = 1000;
        Entity entityHolder = null;

        foreach (var player in playerFilter)
        {
            ref var playerMove = ref player.GetComponent<MoveViewComponent>();
            var distanceToPlayer = Vector2.Distance(Utils.ConvertToV2(enemyMove.transform.position), Utils.ConvertToV2(playerMove.transform.position));
            if (distanceToPlayer <= gameConfig.enemySuspicionRadius)
            {
                if (distanceToPlayer <= minDistance)
                {
                    entityHolder = player;
                    minDistance = distanceToPlayer;
                    
                }
            }
        }

        if (entityHolder != null && minDistance <= gameConfig.enemyAttackRadius)
        {
            enemy.isAttacking = true;
        } else
        {
            enemy.isAttacking = false;
        }
        enemy.aim = entityHolder;
    }
}