using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun.UtilityScripts;
using Morpeh.Globals;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(GameManagerSystem))]
public sealed class GameManagerSystem : UpdateSystem {
    public GlobalEvent createSpectator;

    public GameConfig gameConfig;
    Filter filter;
    Filter playerFilter;

    public override void OnAwake() {
        filter = World.Filter.With<GameManagerComponent>();
        playerFilter = World.Filter.With<PlayerComponent>();
        ref var gameManagerComponent = ref filter.First().GetComponent<GameManagerComponent>();

        //Not used yet
        gameManagerComponent.gameState = GameState.Starting;

        if (!gameManagerComponent.haveLocalPlayer)
        {
            InstantiateNewLocalPlayer(ref gameManagerComponent);
            gameManagerComponent.haveLocalPlayer = true;
        }
        gameManagerComponent.gameState = GameState.Playing;
    }

    private void InstantiateNewLocalPlayer(ref GameManagerComponent gameManagerComponent)
    {
        Vector3 newPlayerPos = ConvertActorToPos(PhotonNetwork.LocalPlayer.ActorNumber);
        var player = PhotonNetwork.Instantiate(gameConfig.playerPrefab.name, newPlayerPos, Quaternion.identity);

        ref var playerData = ref player.GetComponent<MoveViewProvider>().GetData();
        playerData.agent.speed = gameConfig.playerSpeed;
        gameManagerComponent.virtualCamera.m_LookAt = player.transform;
    }

    Vector3 ConvertActorToPos(int actorNumber)
    {
        switch (actorNumber)
        {
            case 1:
                return new Vector3(gameConfig.levelScale.x * 5 - 0.2f, 0.5f, gameConfig.levelScale.y * 5 - 0.2f);
            case 2:
                return new Vector3(-gameConfig.levelScale.x * 5 - 0.2f, 0.5f, -gameConfig.levelScale.y * 5 - 0.2f);
            case 3:
                return new Vector3(gameConfig.levelScale.x * 5 - 0.2f, 0.5f, -gameConfig.levelScale.y * 5 - 0.2f);
            case 4:
                return new Vector3(-gameConfig.levelScale.x * 5 - 0.2f, 0.5f, gameConfig.levelScale.y * 5 - 0.2f);
            default:
                return new Vector3(0, 0.5f, 0);

        }
    }

    public override void OnUpdate(float deltaTime)
    {
        ref var gameManagerComponent = ref filter.First().GetComponent<GameManagerComponent>();
        if (createSpectator.IsPublished)
        {
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("PlayerDied"))
                gameManagerComponent.gameState = GameState.Spectator;
            gameManagerComponent.virtualCamera.m_LookAt = playerFilter.First().GetComponent<MoveViewComponent>().transform;
        }
        if (gameManagerComponent.virtualCamera.m_LookAt == null && playerFilter.Length > 0)
        {
            gameManagerComponent.virtualCamera.m_LookAt = playerFilter.First().GetComponent<MoveViewComponent>().transform;
        }
    }
}