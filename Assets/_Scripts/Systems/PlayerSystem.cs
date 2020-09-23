using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Photon.Pun;
using Morpeh.Globals;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun.UtilityScripts;
using System.Collections.Generic;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(PlayerSystem))]
public sealed class PlayerSystem : UpdateSystem, IInRoomCallbacks
{
    public GlobalEvent leaveRoom;
    public Material[] playerMaterials;

    Filter filter;
    Filter gameManagerFilter;
    Camera camera;

    public override void OnAwake() {
        PhotonNetwork.AddCallbackTarget(this);
        filter = World.Filter.With<PlayerComponent>();
        gameManagerFilter = World.Filter.With<GameManagerComponent>();
        camera = Camera.main;
    }

    public override void Dispose()
    {
        base.Dispose();
        PhotonNetwork.LocalPlayer.CustomProperties = new Hashtable { };
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnUpdate(float deltaTime) {
        if (leaveRoom.IsPublished)
            LeaveRoom();

        foreach (var entity in filter)
        {
            ref var photonViewComponent = ref entity.GetComponent<PhotonViewComponent>();
            var photonView = photonViewComponent.photonView;
            ref var gameManagerComponent = ref gameManagerFilter.First().GetComponent<GameManagerComponent>();

            //Updating all players material if new instance of player was created
            if (filter.Length > gameManagerComponent.playerCount)
            {
                UpdatePlayerMaterials();
                gameManagerComponent.playerCount = (byte)filter.Length;
            }

            if (!photonView.IsMine) continue;

            ref var playerComponent = ref entity.GetComponent<PlayerComponent>();

            //if click, find ray intersection with ground and set this point to player's destination
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hitInfo;
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                bool hit = Physics.Raycast(ray, out hitInfo);
                if (hit)
                {
                    playerComponent.destination = hitInfo.point;
                }
            }
        }
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
        {
            LeaveRoom();
        }
    }

    private static void LeaveRoom()
    {
        if (PhotonNetwork.NetworkClientState != ClientState.Leaving && PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer)
            PhotonNetwork.Disconnect(); //use this method instead LeaveRoom cause it fix bug with leaving room
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        //PhotonNetwork.LoadLevel(0);
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        GetDamage(targetPlayer, changedProps);
        if (Utils.GetLivesPlayerCount() == 1)
            gameManagerFilter.First().GetComponent<GameManagerComponent>().gameState = GameState.Stop;
    }

    private void GetDamage(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("GetDamage"))
        {
            Debug.Log(targetPlayer.ToString() + "got damage");
            foreach (var player in filter)
            {
                ref var photonViewComponent = ref player.GetComponent<PhotonViewComponent>();
                if (photonViewComponent.photonView.Owner == targetPlayer)
                {
                    ref var healthComponent = ref player.GetComponent<HealthComponent>();
                    healthComponent.health--;
                    if (healthComponent.health <= 0)
                    {
                        if (PhotonNetwork.LocalPlayer == photonViewComponent.photonView.Owner)
                            PhotonNetwork.Destroy(photonViewComponent.photonView);
                        if (PhotonNetwork.IsMasterClient && PhotonNetwork.NetworkClientState != ClientState.Leaving)
                            targetPlayer.SetCustomProperties(new Hashtable { { "PlayerDied", true } });
                    }
                }
            }
        }
    }

    private void UpdatePlayerMaterials()
    {
        Debug.Log("Update player materials");
        foreach (var entity in filter)
        {
            ref var player = ref entity.GetComponent<PlayerComponent>();
            ref var photonView = ref entity.GetComponent<PhotonViewComponent>();

            player.meshRenderer.material = playerMaterials[photonView.photonView.Owner.ActorNumber - 1];
        }
    }

    #region not used

    public void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("Master Client was switched");
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("New Player entered  in room");
    }

    public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        Debug.Log("Room properties was updated");
    }
    #endregion
}