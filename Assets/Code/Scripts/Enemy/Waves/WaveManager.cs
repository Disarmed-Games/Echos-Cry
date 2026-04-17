using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public event Action OnNewWaveSpawned, OnWaveSpawningEnded, OnAllWavesCompleted;

    [SerializeField] private float _timeBetweenWaves = 10f;
    [SerializeField] private WaveData[] _allWaves;
    [SerializeField] private NewEnemySpawner _enemySpawner;
    [SerializeField] private IntEventChannel _updateKillCountChannel;

    private int _currentWave = 0;
    private int _totalEnemiesKilled = 0;
    private bool _allWavesCompleted = false;
    private bool _waveHasStarted = false;

    private void Start()
    {
        _enemySpawner.SpawnerID = Guid.NewGuid().GetHashCode();
    }
    private void OnEnable()
    {
        _updateKillCountChannel.Channel += UpdateKillCount;
    }
    private void OnDisable()
    {
        _updateKillCountChannel.Channel -= UpdateKillCount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_waveHasStarted)
        {
            _waveHasStarted = true;
            StartNextWave();
        }
    }

    private int GetTotalEnemiesInWave(WaveData currentWave)
    {
        int count = 0;
        foreach (SpawnData enemySpawns in currentWave.EnemySpawns)
        {
            count += enemySpawns.enemySpawnCount;
        }
        //Debug.Log($"Total enemies in the wave needed to be killed is: {count}");
        return count;
    }
    public void UpdateKillCount(int enemySpawnerID)
    {
        if (enemySpawnerID != _enemySpawner.SpawnerID) return; //If the enemy is not from this wave manager, then don't count it's death.

        if (_allWavesCompleted || !_waveHasStarted) return;

        _totalEnemiesKilled++;
        //Debug.Log($"Kill count of enemy updated to now {_totalEnemiesKilled}");
        if (_totalEnemiesKilled >= GetTotalEnemiesInWave(_allWaves[_currentWave]))
        {
            _currentWave++;
            if (_currentWave >= _allWaves.Length) //All waves completed
            {
                //Debug.Log("Waves Complete!");
                HUDMessage.Instance.UpdateMessage("All Waves Completed!", 2.5f);
                OnAllWavesCompleted?.Invoke();
                _allWavesCompleted = true;
            }
            else
            {
                StartCoroutine(SpawnWaveAfterDelay(_timeBetweenWaves));
            }
        }
    }
    private IEnumerator SpawnWaveAfterDelay(float seconds)
    {
        HUDMessage.Instance.UpdateMessage("New Wave In " + (seconds).ToString() + " Seconds.", 1.5f);
        yield return new WaitForSeconds(seconds);
        StartNextWave();
    }
    public void StartNextWave()
    {
        //Debug.Log($"current wave is {_currentWave}");
        _totalEnemiesKilled = 0;
        
        HUDMessage.Instance.UpdateMessage("Wave " + (_currentWave + 1).ToString() + " Has Begun.", 2f);
        CameraManager.Instance.ScreenShake(0.5f, 2.5f);

        StartCoroutine(SpawnWave(_allWaves[_currentWave]));
    }
    private IEnumerator SpawnWave(WaveData wave)
    {
        OnNewWaveSpawned?.Invoke();

        for (int i = 0; i < wave.EnemySpawns.Length; i++)
        {
            GameObject enemyPrefab = wave.EnemySpawns[i].enemySpawnType;
            EnemyPool pool = EnemyPoolManager.Instance.GetPool(enemyPrefab);
            for (int j = 0; j < wave.EnemySpawns[i].enemySpawnCount; j++)
            {
                Vector3 enemyPosition = _enemySpawner.GetRandomPoint(wave.spawnRadius);
                StartCoroutine(_enemySpawner.SpawnWithDecal(pool, enemyPosition, wave.spawnRadius, (enemy) => { }));
            }
            yield return new WaitForSeconds(wave.spawnInterval);
        }

        OnWaveSpawningEnded?.Invoke();
        yield return null;
    }
}