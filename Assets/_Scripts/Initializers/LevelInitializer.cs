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
        enemyFilter = World.Filter.With<EnemyComponent>();
        playerFilter = World.Filter.With<PlayerComponent>();

        var ground = World.Filter.With<GroundComponent>().Select<GroundComponent>().GetComponent(0);
        ground.transform.localScale = new Vector3(gameConfig.levelScale.x, 1, gameConfig.levelScale.y);
        ground.navMeshSurface.BuildNavMesh();
    }

    public override void Dispose() {
    }
}