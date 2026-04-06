using TMPro;
using UnityEngine;
using DG.Tweening;

public class PlayerProgressBar : MonoBehaviour
{
    [SerializeField] private DoubleFloatEventChannel healthChannel;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private TextMeshProUGUI livesRemainingText;
    [SerializeField] private UnityEngine.UI.Image frontBar;
    [SerializeField] private UnityEngine.UI.Image backBar;
    public float chipSpeed = 2f;
    private float hFraction = 0f;

    void OnEnable()
    {
        if (healthChannel != null) healthChannel.Channel += UpdateBar;
        GameManager.OnPlayerDeathEvent += UpdateLives;
        UpdateLives();
    }

    void OnDisable()
    {
        if (healthChannel != null) healthChannel.Channel -= UpdateBar;
        GameManager.OnPlayerDeathEvent -= UpdateLives;
    }

    private void UpdateLives()
    {
        if (livesRemainingText != null)
            livesRemainingText.text = $"Lives Remaining: {GameManager.PlayerLives}";

        hFraction = 1f;
        frontBar.fillAmount = 1f;
        backBar.fillAmount = 1f;
    }

    private void UpdateBar(float currentValue, float maxValue)
    {
        amountText.text = currentValue.ToString() + "/" + maxValue.ToString();
        hFraction = currentValue / maxValue;
        float fillF = frontBar.fillAmount;
        float fillB = backBar.fillAmount;
    
        if (fillB > hFraction)
        {
            frontBar.fillAmount = hFraction;
            backBar.DOKill();
            DOTween.To(() => backBar.fillAmount, x => backBar.fillAmount = x, hFraction, chipSpeed).SetEase(Ease.OutQuad);
        }
        else if (fillF < hFraction)
        {
            backBar.fillAmount = hFraction;
            frontBar.DOKill();
            DOTween.To(() => frontBar.fillAmount, x => frontBar.fillAmount = x, hFraction, chipSpeed).SetEase(Ease.OutQuad);
        }
        else
        {
            frontBar.fillAmount = hFraction;
            backBar.fillAmount = hFraction;
        }
    }
}
