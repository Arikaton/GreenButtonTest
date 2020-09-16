using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using System.Collections;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(EnemySpawnSystem))]
public sealed class EnemySpawnSystem : UpdateSystem {
    public GameConfig gameConfig;
    Filter filter;

    public override void OnAwake() {
        filter = World.Filter.With<EnemySpawnComponent>();
    }

    public override void OnUpdate(float deltaTime) {
        ref var enemyComponent = ref filter.Select<EnemySpawnComponent>().GetComponent(0);
        if (enemyComponent.timeAfterSpawnPrevEnemy >= gameConfig.enemySpawnDelay)
        {
            var enemy = Instantiate(gameConfig.enemyPrefab);
            enemy.transform.position = new Vector3(
                Random.Range(-gameConfig.levelScale.x * 5, gameConfig.levelScale.x * 5),
                0.35f,
                Random.Range(-gameConfig.levelScale.y * 5, gameConfig.levelScale.y * 5));
            enemyComponent.timeAfterSpawnPrevEnemy = 0;
        } else
        {
            enemyComponent.timeAfterSpawnPrevEnemy += deltaTime;
        }
    }
}