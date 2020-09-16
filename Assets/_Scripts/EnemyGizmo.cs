using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
public class EnemyGizmo : MonoBehaviour
{
    public GameConfig gameConfig;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, gameConfig.enemySuspicionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, gameConfig.enemyAttackRadius);
    }
}
#endif
