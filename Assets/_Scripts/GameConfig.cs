using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Game Config")]
public class GameConfig : ScriptableObject
{
    public Vector2 levelScale;
    public float playerSpeed;
    public float enemySpeed;
    public float enemyAttackRadius;
    public float enemySuspicionRadius;
    public float enemySpawnDelay;
    public float enemyAttackDelay;
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
}
