using Morpeh;
using Morpeh.Globals;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(LauncherSystem))]
public sealed class LauncherSystem : UpdateSystem, IMatchmakingCallbacks, IConnectionCallbacks, IInRoomCallbacks {
    public GlobalEvent startGameEvent;
    Filter filter;

    public override void OnAwake() {
        PhotonNetwork.AddCallbackTarget(this);
        PhotonNetwork.AutomaticallySyncScene = true;

        filter = World.Filter.With<LauncherComponent>();
        ref var launcherComponent = ref filter.Select<LauncherComponent>().GetComponent(0);
        launcherComponent.controlPanel.SetActive(true);
        launcherComponent.loader.SetActive(false);
        launcherComponent.waitPanel.SetActive(false);
    }

    public override void OnUpdate(float deltaTime) {
        if (startGameEvent.IsPublished)
        {
			Connect();
		}
	}

    public override void Dispose()
    {
        base.Dispose();
        Debug.Log("On dispose");
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    void Connect()
	{
        ref var launcherComponent = ref filter.Select<LauncherComponent>().GetComponent(0);
        launcherComponent.isConnecting = true;

		// hide the Play button for visual consistency
		launcherComponent.controlPanel.SetActive(false);
		launcherComponent.loader.SetActive(true);

		// we check if we are connected or not, we join if we are , else we initiate the connection to the server.
		if (PhotonNetwork.IsConnected)
		{
            Debug.Log("Attempt Join Random Room");
			// #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
			PhotonNetwork.JoinRandomRoom();
		}
		else
		{
            Debug.Log("Try connect to master");
			// #Critical, we must first and foremost connect to Photon Online Server.
			PhotonNetwork.ConnectUsingSettings();
			PhotonNetwork.GameVersion = launcherComponent.GameVersion;
		}
	}

    public void OnCreatedRoom()
    {
        Debug.Log("Create room");
    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Create room failed with code " + returnCode + " and message: " + message);
    }

    public void OnFriendListUpdate(List<FriendInfo> friendList)
    {
    }

    public void OnJoinedRoom()
    {
        //LogFeedback("<Color=Green>OnJoinedRoom</Color> with " + PhotonNetwork.CurrentRoom.PlayerCount + " Player(s)");
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.\nFrom here on, your game would be running.");

        // #Critical: We only load if we are the first player, else we rely on  PhotonNetwork.AutomaticallySyncScene to sync our instance scene.
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            // #Critical
            // Load the Room Level. 
            //PhotonNetwork.LoadLevel("GameScene");
            ref var launcherComponent = ref filter.Select<LauncherComponent>().GetComponent(0);
            launcherComponent.waitPanel.SetActive(true);

        }
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("On Join Random Failed");
        ref var launcherComponent = ref filter.Select<LauncherComponent>().GetComponent(0);
        //LogFeedback("<Color=Red>OnJoinRandomFailed</Color>: Next -> Create a new Room");
        //Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = launcherComponent.maxPlayersPerRoom });
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
    }

    public void OnLeftRoom()
    {
    }

    public void OnConnected()
    {
        Debug.Log("On onnected");
    }

    public void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        ref var launcherComponent = ref filter.Select<LauncherComponent>().GetComponent(0);

        if (launcherComponent.isConnecting)
        {
            //LogFeedback("OnConnectedToMaster: Next -> try to Join Random Room");
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room.\n Calling: PhotonNetwork.JoinRandomRoom(); Operation will fail if no room found");

            // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public void OnDisconnected(DisconnectCause cause)
    {
        ref var launcherComponent = ref filter.Select<LauncherComponent>().GetComponent(0);
        //LogFeedback("<Color=Red>OnDisconnected</Color> " + cause);
        Debug.LogError("PUN Basics Tutorial/Launcher:Disconnected");

        // #Critical: we failed to connect or got disconnected. There is not much we can do. Typically, a UI system should be in place to let the user attemp to connect again.
        launcherComponent.loader.SetActive(false);

        launcherComponent.isConnecting = false;
        launcherComponent.controlPanel.SetActive(true);
    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {
        Debug.Log("Region List recieved");
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player Entered Room");
        ref var launcherComponent = ref filter.Select<LauncherComponent>().GetComponent(0);
        if (PhotonNetwork.CurrentRoom.PlayerCount == launcherComponent.maxPlayersPerRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("GameScene");
            }
        }
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
    }

    public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
    }
}