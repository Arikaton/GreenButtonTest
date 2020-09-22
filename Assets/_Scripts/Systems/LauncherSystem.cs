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
    public GlobalEvent leaveRoom;
    public GlobalEvent increasePlayerCount;
    public GlobalEvent reducePlayerCount;

    Filter filter;

    public override void OnAwake()
    {
        PhotonNetwork.AddCallbackTarget(this);
        PhotonNetwork.AutomaticallySyncScene = true;

        filter = World.Filter.With<LauncherComponent>();
        ref var launcherComponent = ref filter.Select<LauncherComponent>().GetComponent(0);

        InitCanvas(ref launcherComponent);
        //Automaticly try to connect to master when game started
        if (!PhotonNetwork.IsConnected)
            ConnectToMaster(ref launcherComponent);
    }

    private static void InitCanvas(ref LauncherComponent launcherComponent)
    {
        launcherComponent.controlPanel.SetActive(true);
        launcherComponent.loader.SetActive(false);
        launcherComponent.waitPanel.SetActive(false);
        launcherComponent.leaveButton.SetActive(false);
        launcherComponent.connectedPlayersCount.text = "";
        launcherComponent.maxPlayerCount.text = launcherComponent.maxPlayersPerRoom.ToString();
    }

    public override void OnUpdate(float deltaTime) {
        ref var launcherComponent = ref filter.Select<LauncherComponent>().GetComponent(0);

        //Update server info
        if (PhotonNetwork.IsConnected)
        {
            launcherComponent.globalPlayerCount.text = PhotonNetwork.CountOfPlayers.ToString();
            launcherComponent.globalRoomsCount.text = PhotonNetwork.CountOfRooms.ToString();
        } else
        {
            launcherComponent.globalPlayerCount.text = "N/A";
            launcherComponent.globalRoomsCount.text = "N/A";
        }

        if (startGameEvent.IsPublished)
        {
			Connect();
		}

        if (leaveRoom.IsPublished)
        {
            PhotonNetwork.LeaveRoom();
        }

        if (increasePlayerCount.IsPublished)
        {
            if (launcherComponent.maxPlayersPerRoom < 4)
            {
                launcherComponent.maxPlayersPerRoom++;
                launcherComponent.maxPlayerCount.text = launcherComponent.maxPlayersPerRoom.ToString();
            }
        }
        if (reducePlayerCount.IsPublished)
        {
            if (launcherComponent.maxPlayersPerRoom > 2)
            {
                launcherComponent.maxPlayersPerRoom--;
                launcherComponent.maxPlayerCount.text = launcherComponent.maxPlayersPerRoom.ToString();
            }
        }
	}

    public override void Dispose()
    {
        base.Dispose();
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    /// <summary>
    /// Connect to server and try joing to room. If Failed - create new room.
    /// </summary>
    void Connect()
	{
        ref var launcherComponent = ref filter.Select<LauncherComponent>().GetComponent(0);
        launcherComponent.isConnecting = true;

		// hide the Play button for visual consistency
		launcherComponent.controlPanel.SetActive(false);
		launcherComponent.loader.SetActive(true);
        launcherComponent.leaveButton.SetActive(true);

		// we check if we are connected or not, we join if we are , else we initiate the connection to the server.
		if (PhotonNetwork.IsConnected)
		{
            Debug.Log("Attempt Join Random Room");
			// #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
			PhotonNetwork.JoinRandomRoom(new Hashtable { }, launcherComponent.maxPlayersPerRoom);
		}
		else
        {
            ConnectToMaster(ref launcherComponent);
        }
    }

    private static void ConnectToMaster(ref LauncherComponent launcherComponent)
    {
        Debug.Log("Try connect to master");
        // #Critical, we must first and foremost connect to Photon Online Server.
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = launcherComponent.GameVersion;
    }

    public void OnCreatedRoom()
    {
        Debug.Log("Create room");
        UpdatePlayerCountText();
    }

    public void OnJoinedRoom()
    {
        UpdatePlayerCountText();
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            ref var launcherComponent = ref filter.Select<LauncherComponent>().GetComponent(0);
            launcherComponent.waitPanel.SetActive(true);
        }
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("On Join Random Failed");
        ref var launcherComponent = ref filter.Select<LauncherComponent>().GetComponent(0);
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = launcherComponent.maxPlayersPerRoom });
    }

    public void OnLeftRoom()
    {
        ref var launcherComponent = ref filter.Select<LauncherComponent>().GetComponent(0);
        launcherComponent.isConnecting = false;
        InitCanvas(ref launcherComponent);
    }

    void UpdatePlayerCountText()
    {
        ref var launcherComponent = ref filter.Select<LauncherComponent>().GetComponent(0);
        launcherComponent.connectedPlayersCount.text = PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    public void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        ref var launcherComponent = ref filter.Select<LauncherComponent>().GetComponent(0);

        if (launcherComponent.isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public void OnDisconnected(DisconnectCause cause)
    {
        ref var launcherComponent = ref filter.Select<LauncherComponent>().GetComponent(0);

        launcherComponent.loader.SetActive(false);
        launcherComponent.controlPanel.SetActive(true);
        launcherComponent.isConnecting = false;
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerCountText();
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
        UpdatePlayerCountText();
    }

    #region not used

    public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
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

    public void OnConnected()
    {
        Debug.Log("On Connected");
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Create room failed with code " + returnCode + " and message: " + message);
    }

    public void OnFriendListUpdate(List<FriendInfo> friendList)
    {
    }

    #endregion
}