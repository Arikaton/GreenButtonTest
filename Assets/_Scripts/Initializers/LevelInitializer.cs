using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Initializers/" + nameof(LevelInitializer))]
public sealed class LevelInitializer : Initializer {
    public GameConfig gameConfig;
    Filter playerFilter;
    Filter enemyFilter;

    public override void OnAwake() {
        Filter commonFilter = World.Filter.With<MoveViewComponent>();
        enemyFilter = commonFilter.With<EnemyComponent>();
        playerFilter = commonFilter.With<PlayerComponent>();

        var ground = World.Filter.With<GroundComponent>().Select<GroundComponent>().GetComponent(0);
        ground.transform.localScale = new Vector3(gameConfig.levelScale.x, 1, gameConfig.levelScale.y);

        foreach (var playerEntity in playerFilter)
        {
            ref var playerMove = ref playerEntity.GetComponent<MoveViewComponent>();
            playerMove.agent.speed = gameConfig.playerSpeed;
        }

        foreach (var enemyEntity in enemyFilter)
        {
            ref var enemyMove = ref enemyEntity.GetComponent<MoveViewComponent>();
            ref var enemy = ref enemyEntity.GetComponent<EnemyComponent>();
            enemy.timeAfterLastAttack = float.MaxValue;
            enemyMove.agent.speed = gameConfig.enemySpeed;
            
        }
    }

    public override void Dispose() {
        Debug.Log("On Dispose");
    }
}