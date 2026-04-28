using TMPro;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.Assertions;
using Ink.Parsed;

public class PlayerProgressBar : MonoBehaviour
{
    private enum Bar
    {
        HEALTH,
        ARMOR,
    }
    [SerializeField] private Bar barType;

    [SerializeField] private DoubleFloatEventChannel healthChannel;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private TextMeshProUGUI livesRemainingText;
    [SerializeField] private UnityEngine.UI.Image frontBar;
    [SerializeField] private UnityEngine.UI.Image backBar;

    public float lerpSpeed = 0.5f;
    private float frontVelocity;
    private float backVelocity;
    private float targetFraction = 1f;
    private HealthSystem playerHealth = null;

    void OnEnable()
    {
        if (healthChannel != null) healthChannel.Channel += UpdateBar;
        GameManager.OnPlayerDeathEvent += UpdateLives;

        if (barType == Bar.HEALTH)
        {
            if (playerHealth == null)
                playerHealth = FindAnyObjectByType<HealthSystem>();
            
            if (playerHealth != null)
                UpdateBar(playerHealth.CurrentHealth, playerHealth.MaxHealth);
        }
        else if (barType == Bar.ARMOR)
        {
            if (playerHealth == null)
                playerHealth = FindAnyObjectByType<HealthSystem>();

            if (playerHealth != null)  
                UpdateBar(playerHealth.CurrentArmor, playerHealth.MaxArmor);  
        }

        UpdateLives();
    }

    void OnDisable()
    {
        if (healthChannel != null) healthChannel.Channel -= UpdateBar;
        GameManager.OnPlayerDeathEvent -= UpdateLives;
    }

    private void UpdateLives()
    {
        if (barType == Bar.HEALTH)
            livesRemainingText.text = $"Lives Remaining: {GameManager.PlayerLives}";

        targetFraction = 1f;
        frontBar.fillAmount = 1f;
        backBar.fillAmount = 1f;
    }

    private void UpdateBar(float currentValue, float maxValue)
    {
        amountText.text = $"{currentValue}/{maxValue}";
        targetFraction = currentValue / maxValue;
    }
    void Update()
    {
        frontBar.fillAmount = Mathf.SmoothDamp(
            frontBar.fillAmount,
            targetFraction,
            ref frontVelocity,
            lerpSpeed
        );

        backBar.fillAmount = Mathf.SmoothDamp(
            backBar.fillAmount,
            targetFraction,
            ref backVelocity,
            lerpSpeed * 1.5f
        );
    }
}
