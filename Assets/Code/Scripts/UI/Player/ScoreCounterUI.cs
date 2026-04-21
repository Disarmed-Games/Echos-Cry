using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] float timeToMax = 2f;
    [SerializeField] float timePerStep = 0.1f;
    [SerializeField] private IntEventChannel textEvent;
    [SerializeField] private Animator _animator;
    private readonly int bounceHash = Animator.StringToHash("Bounce");
    float divisor;
    private int countPerStep = 0;
    private int currentScore = 0;
    private int targetScore = 0;

    private void Start()
    {
        divisor = timeToMax / timePerStep;
        text.text = "Score: 0";
    }
    private void OnEnable()
    {
        textEvent.Channel += UpdateText;
    }
    private void OnDisable()
    {
        textEvent.Channel -= UpdateText;
    }

    public void UpdateText(int score)
    {
        targetScore = score;
        countPerStep = (int)((targetScore - currentScore) / divisor);
        StopAllCoroutines();
        StartCoroutine(CounterCoroutine());
    }
    IEnumerator CounterCoroutine()
    {
        while (true)
        {
            currentScore += countPerStep;
            if (currentScore > targetScore)
            {
                currentScore = targetScore;
                text.text = "Score: " + currentScore.ToString();
                _animator.Play(bounceHash);
                break;
            }
            text.text = "Score: " + currentScore.ToString();
            _animator.Play(bounceHash);
            yield return new WaitForSeconds(timePerStep);
        }
    }
}
