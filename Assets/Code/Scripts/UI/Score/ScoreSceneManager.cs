using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using static ScoreSceneManager;

public class ScoreSceneManager : MonoBehaviour
{
    [SerializeField] private SceneField sceneTarget;
    [SerializeField] private StarUIHandler[] starImages = new StarUIHandler[5];
    [SerializeField] private TextMeshProUGUI ratingText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI xpText;

    [SerializeField] float timeToMax = 1f;
    [SerializeField] float timePerStep = 0.05f;
    float divisor;

    private float transitionTime = 0.3f;
    private bool triggerRating = true;

    [SerializeField] private int maxXPBonus;
    [SerializeField] private int maxGoldBonus;
    int xpBonus = 0;
    int goldBonus = 0;

    //Rating Variables
    public enum Rank
    {
        D = 0,
        C = 1,
        B = 2,
        A = 3,
        SPlus = 4
    }
    Rank calculatedRank = Rank.SPlus;

    public static string GetRankString(Rank rank)
    {
        switch (rank)
        {
            case Rank.SPlus: return "S+";
            case Rank.A: return "A";
            case Rank.B: return "B";
            case Rank.C: return "C";
            case Rank.D: return "D";
            default: return "?";
        }
    }
    public void ContinueButton()
    {
        GameManager.Instance.SceneManager.TransitionScene(sceneTarget, GameManager.Instance);
        MenuManager.Instance.DisablePauseMenu();
    }
    private void OnEnable()
    {
        divisor = timeToMax / timePerStep;
        ratingText.text = "?";
        scoreText.text = "0";
        goldText.text = "?";
        xpText.text = "?";
        triggerRating = true;

        //Calculate Stats
        int score = ScoreManager.Instance.CurrentScore;
        int topScore = ScoreManager.Instance.TopScore;

        float percent = (float)score / topScore;
        if (percent >= 0.9f)    calculatedRank = Rank.SPlus;
        else if (percent >= 0.8f)    calculatedRank = Rank.A;
        else if (percent >= 0.7f)    calculatedRank = Rank.B;
        else if (percent >= 0.6f)    calculatedRank = Rank.C;
        else                    calculatedRank = Rank.D;

        float bonusMultiplier = Mathf.Min(1f, percent + 0.1f);
        xpBonus = (int)Mathf.Floor(maxXPBonus * bonusMultiplier);
        goldBonus = (int)Mathf.Floor(maxGoldBonus * bonusMultiplier);

        Player.Instance.XP.IncreaseXP(xpBonus);
        Player.Instance.CurrencySystem.IncrementGoldCurrency(goldBonus);

        StopAllCoroutines();
        DisplayScore(score);
    }
    private void DisplayScore(int score)
    {
        int currentScore = 0;
        int targetScore = score;
        int countPerStep = (int)((targetScore - currentScore) / divisor);
        StartCoroutine(CounterCoroutine(scoreText, currentScore, targetScore, countPerStep));
    }
    private void DisplayRating()
    {
        StartCoroutine(StarCoroutine(calculatedRank));
    }

    private void DisplayBonus()
    {
        int currentScore = 0;
        int targetScore = xpBonus;
        int countPerStep = (int)((targetScore - currentScore) / divisor);
        StartCoroutine(CounterCoroutine(xpText, currentScore, targetScore, countPerStep));

        currentScore = 0;
        targetScore = goldBonus;
        countPerStep = (int)((targetScore - currentScore) / divisor);
        StartCoroutine(CounterCoroutine(goldText, currentScore, targetScore, countPerStep));
    }
    IEnumerator StarCoroutine(Rank rank)
    {
        for (int i = 0; i <= (int)rank; i++)
        {
            starImages[i].UnlockStar();
            Transform starTransform = starImages[i].GetComponent<Transform>();
            starTransform.DOKill();
            starTransform.localRotation = Quaternion.identity;
            starTransform.DOShakeRotation(transitionTime, 30, 10, 45, true);

            yield return new WaitForSeconds(0.5f);
        }

        ratingText.text = GetRankString(rank);
        Transform ratingtTransform = ratingText.GetComponent<Transform>();
        ratingtTransform.DOKill();
        ratingtTransform.localRotation = Quaternion.identity;
        ratingtTransform.DOShakeRotation(transitionTime, 30, 10, 45, true);

        DisplayBonus();
    }
    IEnumerator CounterCoroutine(TextMeshProUGUI textObj, int currentScore, int targetScore, int countPerStep)
    {
        while (true)
        {
            currentScore += countPerStep;
            if (currentScore > targetScore)
            {
                currentScore = targetScore;
                textObj.text = currentScore.ToString();
                break;
            }
            textObj.text = currentScore.ToString();
            yield return new WaitForSeconds(timePerStep);
        }

        if (triggerRating)
        {
            triggerRating = false;
            DisplayRating();
        }
    }
}
