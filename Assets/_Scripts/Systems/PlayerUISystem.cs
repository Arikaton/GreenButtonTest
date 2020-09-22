using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Photon.Pun;
using Morpeh.Globals;
using Photon.Realtime;
using ExitGames.Client.Photon;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(PlayerUISystem))]
public sealed class PlayerUISystem : UpdateSystem, IInRoomCallbacks {
    public GlobalEvent createSpectator;

    Filter playerFilter;
    Filter uiFilter;

    public override void OnAwake() {
        PhotonNetwork.AddCallbackTarget(this);

        playerFilter = World.Filter.With<PlayerComponent>();
        uiFilter = World.Filter.With<PlayerUIComponent>();

        ref var uiComponent = ref uiFilter.Select<PlayerUIComponent>().GetComponent(0);
        //Not really matter which entity has that component, cause all players have eq health in begin;
        ref var healthComponent = ref playerFilter.Select<HealthComponent>().GetComponent(0); 
        uiComponent.healthSlider.maxValue = healthComponent.health;
        uiComponent.winPopUp.SetActive(false);
        uiComponent.losePopUp.SetActive(false);
    }

    public override void Dispose()
    {
        base.Dispose();
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnUpdate(float deltaTime) {
        var healthBug = playerFilter.Select<HealthComponent>();
        var photonViewBug = playerFilter.Select<PhotonViewComponent>();
        ref var uiComponent = ref uiFilter.Select<PlayerUIComponent>().GetComponent(0);

        if (createSpectator.IsPublished)
        {
            uiComponent.losePopUp.SetActive(false);
        }

        for (int i = 0, lenght = playerFilter.Length; i < lenght; i++)
        {
            ref var healthComponent = ref healthBug.GetComponent(i);

            ref var photonView = ref photonViewBug.GetComponent(i);
            if (!photonView.photonView.IsMine) continue;

            uiComponent.healthSlider.value = healthComponent.health;
        }
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        ref var uiComponent = ref uiFilter.Select<PlayerUIComponent>().GetComponent(0);
        if (changedProps.ContainsKey("PlayerDied"))
        {
            if (targetPlayer.IsLocal && !uiComponent.winPopUp.activeSelf)
            {
                uiComponent.healthSlider.value = 0;
                uiComponent.losePopUp.SetActive(true);
            } else
            {
                int livePlayerCount = Utils.GetLivesPlayerCount();
                Debug.Log("Lives player count: " + livePlayerCount);
                if (livePlayerCount == 1 && !uiComponent.losePopUp.activeSelf && !PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("PlayerDied"))
                {
                    uiComponent.winPopUp.SetActive(true);
                }
            }
        }
    }

    #region not used 
    public void OnMasterClientSwitched(Player newMasterClient)
    {
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
    }

    

    public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
    }
    #endregion
}