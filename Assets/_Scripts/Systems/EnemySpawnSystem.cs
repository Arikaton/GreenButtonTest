using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using System.Collections;
using Photon.Pun;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(EnemySpawnSystem))]
public sealed class EnemySpawnSystem : UpdateSystem {
    public GameConfig gameConfig;

    Filter filter;
    Filter gameManagerFilter;

    public override void OnAwake() {
        filter = World.Filter.With<EnemySpawnComponent>();
        gameManagerFilter = World.Filter.With<GameManagerComponent>();
    }

    public override void OnUpdate(float deltaTime) {
        if (!PhotonNetwork.IsMasterClient) return;
        if (gameManagerFilter.First().GetComponent<GameManagerComponent>().gameState != GameState.Playing) return;

        ref var enemySpanwComponent = ref filter.Select<EnemySpawnComponent>().GetComponent(0);
        if (enemySpanwComponent.timeAfterSpawnPrevEnemy >= gameConfig.enemySpawnDelay)
        {
            var newEnemyPos = new Vector3(
                Random.Range(-gameConfig.levelScale.x * 5, gameConfig.levelScale.x * 5),
                0.35f,
                Random.Range(-gameConfig.levelScale.y * 5, gameConfig.levelScale.y * 5));
            var enemy = PhotonNetwork.Instantiate(gameConfig.enemyPrefab.name, newEnemyPos, Quaternion.identity);
            ref var enemyMoveData = ref enemy.GetComponent<MoveViewProvider>().GetData();

            enemyMoveData.agent.speed = gameConfig.enemySpeed;
            enemySpanwComponent.timeAfterSpawnPrevEnemy = 0;
        } else
        {
            enemySpanwComponent.timeAfterSpawnPrevEnemy += deltaTime;
        }
    }
}