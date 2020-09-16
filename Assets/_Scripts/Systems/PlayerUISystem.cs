using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Photon.Pun;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(PlayerUISystem))]
public sealed class PlayerUISystem : UpdateSystem {
    Filter playerFilter;
    Filter uiFilter;
    public override void OnAwake() {
        playerFilter = World.Filter.With<PlayerComponent>();
        uiFilter = World.Filter.With<PlayerUIComponent>();

        ref var uiComponent = ref uiFilter.Select<PlayerUIComponent>().GetComponent(0);
        ref var healthComponent = ref playerFilter.Select<HealthComponent>().GetComponent(0); //Not really matter which entity has that component, cause all players have eq health in begin;
        uiComponent.healthSlider.maxValue = healthComponent.health;
    }

    public override void OnUpdate(float deltaTime) {
        var healthBug = playerFilter.Select<HealthComponent>();
        var photonViewBug = playerFilter.Select<PhotonViewComponent>();
        ref var uiComponent = ref uiFilter.Select<PlayerUIComponent>().GetComponent(0);
        for (int i = 0, lenght = playerFilter.Length; i < lenght; i++)
        {
            ref var photonView = ref photonViewBug.GetComponent(i);
            if (!photonView.photonView.IsMine) continue;

            ref var healthComponent = ref healthBug.GetComponent(i);
            uiComponent.healthSlider.value = healthComponent.health;

        }
    }
}