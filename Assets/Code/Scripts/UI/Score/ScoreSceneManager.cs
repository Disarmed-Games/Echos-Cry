using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;

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

    //Rating Variables
    public enum Rank
    {
        D = 0,
        C = 1,
        B = 2,
        A = 3,
        SPlus = 4
    }
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
        StopAllCoroutines();
        DisplayScore();
    }
    private void DisplayScore()
    {
        int currentScore = 0;
        int targetScore = ScoreManager.Instance.CurrentScore;
        int countPerStep = (int)((targetScore - currentScore) / divisor);
        StartCoroutine(CounterCoroutine(scoreText, currentScore, targetScore, countPerStep));
    }
    private void CalculateAndDisplayRating()
    {
        //Function to estimate rating
        Rank calculatedRank = Rank.SPlus;
        StartCoroutine(StarCoroutine(calculatedRank));
    }

    private void CalculateAndDisplayBonus(Rank rank)
    {
        //Calculate bonus based on rank
        int xpBonus = 0;
        int goldBonus = 0;
        switch (rank)
        {
            case Rank.SPlus:
                xpBonus = 10;
                goldBonus = 5;
                break;
            case Rank.A:
                xpBonus = 25;
                goldBonus = 8;
                break;
            case Rank.B:
                xpBonus = 50;
                goldBonus = 10;
                break;
            case Rank.C:
                xpBonus = 100;
                goldBonus = 15;
                break;
            case Rank.D:
                xpBonus = 150;
                goldBonus = 25;
                break;
        }

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

        CalculateAndDisplayBonus(rank);
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
            CalculateAndDisplayRating();
        }
    }
}
