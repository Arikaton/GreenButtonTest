using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using UnityEngine.AI;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[System.Serializable]
public struct MoveViewComponent : IComponent
{
    public NavMeshAgent agent;
    public Transform transform;
}