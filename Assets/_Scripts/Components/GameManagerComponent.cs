﻿using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[System.Serializable]
public struct GameManagerComponent : IComponent {
    public byte playerCount;
    public bool haveLocalPlayer;
    public Cinemachine.CinemachineVirtualCamera virtualCamera;
    public GameState gameState;
}

public enum GameState
{
    Starting,
    Playing,
    Stop,
    Spectator
}