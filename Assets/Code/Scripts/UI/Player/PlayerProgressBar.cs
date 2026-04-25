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
    public float chipSpeed = 2f;
    private float targetFraction;
    private HealthSystem playerHealth = null;

    void OnEnable()
    {
        if (healthChannel != null) healthChannel.Channel += UpdateBar;
        GameManager.OnPlayerDeathEvent += UpdateLives;

        if (barType == Bar.HEALTH)
        {
            //use stored reference rather than finding the object every time on death reset 
            if (playerHealth == null)
            {
                playerHealth = FindAnyObjectByType<HealthSystem>();
            }
            
            if (playerHealth != null) {
                
                //Debug.Log($"in progress bar curr health: {playerHealth.CurrentHealth}, max plyr health: {playerHealth.MaxHealth}");
                UpdateBar(playerHealth.CurrentHealth, playerHealth.MaxHealth);
            }
        }
        else if (barType == Bar.ARMOR)
        {
            if (playerHealth == null)
            {
                playerHealth = FindAnyObjectByType<HealthSystem>();
            }

            if (playerHealth != null)
            {
                
                //Debug.Log($"in progress bar curr armor: {playerHealth.CurrentArmor}, max plyr armor: {playerHealth.MaxArmor}");
                UpdateBar(playerHealth.CurrentArmor, playerHealth.MaxArmor);
            }    
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
        //Debug.Log($"Current value: {currentValue}, maxValue: {maxValue}");
        amountText.text = $"{currentValue}/{maxValue}";

        targetFraction = currentValue / maxValue;

        backBar.DOKill();
        frontBar.DOKill();

        float frontFill = frontBar.fillAmount;
        float backFill = backBar.fillAmount;

        if (backFill > targetFraction)
        {
            frontBar.fillAmount = targetFraction;
            DOTween.To(() => backBar.fillAmount, x => backBar.fillAmount = x, targetFraction, chipSpeed).SetEase(Ease.OutQuad);
        }
        else if (frontFill < targetFraction)
        {
            backBar.fillAmount = targetFraction;
            DOTween.To(() => frontBar.fillAmount, x => frontBar.fillAmount = x, targetFraction, chipSpeed).SetEase(Ease.OutQuad);
        }
        else
        {
            frontBar.fillAmount = targetFraction;
            backBar.fillAmount = targetFraction;
        }
    }
}
