﻿using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(GameManagerSystem))]
public sealed class GameManagerSystem : UpdateSystem, IInRoomCallbacks {
    public GameConfig gameConfig;
    Filter filter;

    public override void OnAwake() {
        PhotonNetwork.AddCallbackTarget(this);

        filter = World.Filter.With<GameManagerComponent>();
        Debug.Log("Game Manager: " + filter.Length);
        ref var gameManagerComponent = ref filter.First().GetComponent<GameManagerComponent>();
        if (!gameManagerComponent.haveLocalPlayer)
        {
            Vector3 newPlayerPos;
            if (PhotonNetwork.IsMasterClient)
            {
                newPlayerPos = new Vector3(gameConfig.levelScale.x * 5 - 0.2f, 0.5f, gameConfig.levelScale.y * 5 - 0.2f);
            } else
            {
                newPlayerPos = new Vector3(-gameConfig.levelScale.x * 5 - 0.2f, 0.5f, -gameConfig.levelScale.y * 5 - 0.2f);
            }
            var player = PhotonNetwork.Instantiate(gameConfig.playerPrefab.name, newPlayerPos, Quaternion.identity);
            gameManagerComponent.virtualCamera.m_LookAt = player.transform;
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnUpdate(float deltaTime) {
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("Master client switched");
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("<color=green>Game Manager:</color> Player Entered Room");
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("<color=green>GameManager: </color>player {0} left room, players count now is - {1}", otherPlayer.NickName, PhotonNetwork.CurrentRoom.PlayerCount);
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            PhotonNetwork.LeaveRoom();
        }
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        Debug.Log("Player properties Updated");
    }

    public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        Debug.Log("Player properties updated");
    }
}