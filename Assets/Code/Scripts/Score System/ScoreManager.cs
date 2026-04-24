using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : NonSpawnableSingleton<ScoreManager>
{
    private int currentScore = 0;
    private int topScore; //TODO: This needs to be updated per level
    [SerializeField] GameObject scoreTextPrefab;
    [SerializeField] IntEventChannel textUpdate;

    public int CurrentScore { get => currentScore; }
    public int TopScore {  get => topScore; }
    public void AddScore(int score)
    {
        currentScore += score;
        textUpdate.Invoke(currentScore);
    }
    public void ResetScore()
    {
        currentScore = 0;
        textUpdate.Invoke(currentScore);
    }
    //Calculate score based on attackinfo at death of enemy and base score
    public int CalculateScore(AttackInfo info, int baseScore)
    {

        float qualityMultiplier = 0;
        switch (info.HitQuality)
        {
            case TempoConductor.HitQuality.Excellent:
                qualityMultiplier = 3.0f;
                break;
            case TempoConductor.HitQuality.Good:
                qualityMultiplier = 1.5f;
                break;
            case TempoConductor.HitQuality.Miss:
                qualityMultiplier = 0.75f;
                break;
        }
        return (int)((baseScore * qualityMultiplier) + info.Damage * 100);
    }
    public void SpawnScoreText(int score, Transform origin)
    {
        ScoreTextUI text = Instantiate(scoreTextPrefab).GetComponent<ScoreTextUI>();
        text.transform.position = origin.position;
        text.TextMesh.text = "+" + score.ToString();
    }

    void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        topScore = 0;
        WaveManager[] waveManagers = Object.FindObjectsByType<WaveManager>(FindObjectsSortMode.None);
        if (waveManagers == null || waveManagers.Length == 0) return;

        foreach (WaveManager wm in waveManagers)
        {
            foreach (WaveData wave in wm.AllWaves)
            {
                topScore += wm.GetTotalEnemiesInWave(wave) * 1000;
            }
        }
        Debug.Log(topScore);
    }
}
