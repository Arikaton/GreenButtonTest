﻿using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using UnityEngine.UI;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[System.Serializable]
public struct LauncherComponent : IComponent {
    public bool isConnecting;
    public string GameVersion;
    public byte maxPlayersPerRoom;
    public GameObject controlPanel;
    public GameObject loader;
    public GameObject waitPanel;
    public Text connectedPlayersCount;
    public Text globalPlayerCount;
    public Text globalRoomsCount;
    public Text maxPlayerCount;
    public GameObject leaveButton;
}