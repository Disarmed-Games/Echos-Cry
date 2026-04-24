using UnityEngine;


[System.Serializable]
public struct SpawnData
{
    public GameObject enemySpawnType;
    public int enemySpawnCount;
}

[CreateAssetMenu(fileName = "WaveData", menuName = "Echo's Cry/Enemy Wave System/WaveData")]
public class WaveData : ScriptableObject
{
    [Tooltip("List of every enemy type and how many will spawn in a given wave.")] 
    public SpawnData[] EnemySpawns;
    public float spawnInterval; 
}
