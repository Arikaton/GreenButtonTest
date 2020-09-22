using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Morpeh.Globals;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(LaunchCubeSystem))]
public sealed class LaunchCubeSystem : UpdateSystem {
    public GlobalEvent startGame;
    Filter filter;

    public override void OnAwake() {
        filter = World.Filter.With<LaunchCubeComponent>();
        foreach (var cube in filter)
        {
            ref var comp = ref cube.GetComponent<LaunchCubeComponent>();
            comp.startYPos = comp.transform.position.y;
            comp.amplitude = Random.Range(-1.5f, 1.5f);
        }
    }

    //Some dumb code below
    public override void OnUpdate(float deltaTime) {
        var compBag = filter.Select<LaunchCubeComponent>();

        for (int i = 0, lenght = filter.Length; i < lenght; i++)
        {
            ref var launcherComp = ref compBag.GetComponent(i);
            var newY = launcherComp.startYPos + launcherComp.amplitude * Mathf.Sin(Time.realtimeSinceStartup);
            launcherComp.transform.position = new Vector3(
                launcherComp.transform.position.x,
                newY,
                launcherComp.transform.position.z);
        }
    }
}