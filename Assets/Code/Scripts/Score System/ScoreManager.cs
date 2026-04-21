using UnityEngine;

public class ScoreManager : NonSpawnableSingleton<ScoreManager>
{
    private int currentScore = 0;
    [SerializeField] GameObject scoreTextPrefab;
    [SerializeField] IntEventChannel textUpdate;

    public int CurrentScore { get => currentScore; }
    public void AddScore(int score)
    {
        currentScore += score;
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
}
