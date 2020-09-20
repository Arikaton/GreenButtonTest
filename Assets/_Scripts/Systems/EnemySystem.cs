using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using System.Collections.Generic;
using Photon.Pun;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(EnemySystem))]
public sealed class EnemySystem : UpdateSystem {
    public GameConfig gameConfig;
    Filter enemyFilter;
    Filter playerFilter;

    public override void OnAwake() {
        enemyFilter = World.Filter.With<EnemyComponent>();
        playerFilter = World.Filter.With<PlayerComponent>();
    }

    public override void OnUpdate(float deltaTime) {
        //if (!PhotonNetwork.IsMasterClient) return;

        var enemiesBag = enemyFilter.Select<EnemyComponent>();
        var moveBag = enemyFilter.Select<MoveViewComponent>();

        for (int i = 0, lenght = enemyFilter.Length; i < lenght; i++)
        {
            ref var enemy = ref enemiesBag.GetComponent(i);
            ref var enemyMove = ref moveBag.GetComponent(i);

            TryFindPlayer(ref enemy, ref enemyMove);
        }
    }

    private void TryFindPlayer(ref EnemyComponent enemy, ref MoveViewComponent enemyMove)
    {
        float minDistance = 1000;
        Entity entityHolder = null;

        foreach (var player in playerFilter)
        {
            ref var playerMove = ref player.GetComponent<MoveViewComponent>();
            ref var playerHealthComponent = ref player.GetComponent<HealthComponent>();
            if (playerHealthComponent.health <= 0) continue;

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