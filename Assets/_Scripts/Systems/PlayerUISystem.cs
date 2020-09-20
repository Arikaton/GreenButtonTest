using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Photon.Pun;
using Morpeh.Globals;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(PlayerUISystem))]
public sealed class PlayerUISystem : UpdateSystem {
    public GlobalEvent playerDied;

    Filter playerFilter;
    Filter uiFilter;
    public override void OnAwake() {
        playerFilter = World.Filter.With<PlayerComponent>();
        uiFilter = World.Filter.With<PlayerUIComponent>();

        ref var uiComponent = ref uiFilter.Select<PlayerUIComponent>().GetComponent(0);
        //Not really matter which entity has that component, cause all players have eq health in begin;
        ref var healthComponent = ref playerFilter.Select<HealthComponent>().GetComponent(0); 
        uiComponent.healthSlider.maxValue = healthComponent.health;
        uiComponent.winPopUp.SetActive(false);
        uiComponent.losePopUp.SetActive(false);
    }

    public override void OnUpdate(float deltaTime) {
        var healthBug = playerFilter.Select<HealthComponent>();
        var photonViewBug = playerFilter.Select<PhotonViewComponent>();
        ref var uiComponent = ref uiFilter.Select<PlayerUIComponent>().GetComponent(0);
        Debug.Log("PlayerFilter lenght is " + playerFilter.Length);

        for (int i = 0, lenght = playerFilter.Length; i < lenght; i++)
        {
            ref var healthComponent = ref healthBug.GetComponent(i);
            if (playerDied.IsPublished)
            {
                if (healthComponent.health <= 0)
                {
                    uiComponent.losePopUp.SetActive(true);
                } else
                {
                    if (playerFilter.Length <= 2 && !uiComponent.losePopUp.activeSelf)
                    {
                        uiComponent.winPopUp.SetActive(true);
                    }
                }
            }

            ref var photonView = ref photonViewBug.GetComponent(i);
            if (!photonView.photonView.IsMine) continue;

            //ref var healthComponent = ref healthBug.GetComponent(i);
            uiComponent.healthSlider.value = healthComponent.health;

            if (healthComponent.health <= 0)
            {
                uiComponent.losePopUp.SetActive(true);
            }
        }

        /*if (playerDied.IsPublished)
        {
            if (playerFilter.Length == 1 && !uiComponent.losePopUp.activeSelf)
            {
                uiComponent.winPopUp.SetActive(true);
                return;
            }
        }*/
    }
}