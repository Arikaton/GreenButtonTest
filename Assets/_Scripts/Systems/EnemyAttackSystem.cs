using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Photon.Pun;
using Morpeh.Globals;
using ExitGames.Client.Photon;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(EnemyAttackSystem))]
public sealed class EnemyAttackSystem : UpdateSystem {
    public GameConfig gameConfig;

    Filter enemyFilter;

    public override void OnAwake() {
        enemyFilter = World.Filter.With<EnemyComponent>();
    }

    public override void OnUpdate(float deltaTime) {
        if (!PhotonNetwork.IsMasterClient) return;
        var enemyBag = enemyFilter.Select<EnemyComponent>();

        for (int i = 0, lenght = enemyFilter.Length; i < lenght; i++)
        {
            ref var enemy = ref enemyBag.GetComponent(i);
            if (enemy.isAttacking)
            {
                if (enemy.timeAfterLastAttack >= gameConfig.enemyAttackDelay)
                {
                    Hashtable hash = new Hashtable
                    {
                        {"GetDamage", true }
                    };
                    enemy.aim.GetComponent<PhotonViewComponent>().photonView.Owner.SetCustomProperties(hash);

                    enemy.timeAfterLastAttack = 0;
                }
            }

            enemy.timeAfterLastAttack += deltaTime;
        }
    }
}