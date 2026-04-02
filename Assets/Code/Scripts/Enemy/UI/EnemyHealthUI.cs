using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class EnemyHealthUI : MonoBehaviour
{
    [SerializeField] private Image _healthBarFront;
    [SerializeField] private Image _healthBarBack;
    [SerializeField] private Image _shieldBar;
    [SerializeField] private Color _frontFlashColor;
    [SerializeField] private float chipSpeed = 3f;
    
    public void UpdateUI(float currentHealth, float maxHealth, float currentShield, float maxShield)
    {
        float sFraction = currentShield / maxShield;
        _shieldBar.fillAmount = sFraction;

        float hFraction = currentHealth / maxHealth;
        float fillF = _healthBarFront.fillAmount;
        float fillB = _healthBarBack.fillAmount;
        //lerpTimer = 0f;
        if (fillB > hFraction)
        {
            _healthBarFront.fillAmount = hFraction;
            _healthBarFront.DOKill();
            DOTween.To(() => _healthBarFront.color, x => { _frontFlashColor = x; }, _frontFlashColor, 0.10f).SetEase(Ease.OutQuad);
            _healthBarBack.DOKill();
            DOTween.To(() => _healthBarBack.fillAmount, x => _healthBarBack.fillAmount = x, hFraction, chipSpeed).SetEase(Ease.OutQuad);
        }
        if (fillF < hFraction)
        {
            _healthBarBack.fillAmount = hFraction;
            _healthBarBack.color = Color.green;
            _healthBarFront.DOKill();
            DOTween.To(() => _healthBarFront.fillAmount, x => _healthBarFront.fillAmount = x, hFraction, chipSpeed).SetEase(Ease.OutQuad);
        }
    }

    public void ResetUI(float currentHealth, float maxHealth, float currentShield, float maxShield)
    {
        float sFraction = 1;
        float hFraction = 1;
        if(currentShield > 0 && maxShield > 0) sFraction = currentShield / maxShield;
        if (currentHealth > 0 && maxHealth > 0) hFraction = currentHealth / maxHealth;

        _healthBarBack.fillAmount = hFraction;
        _healthBarFront.fillAmount = hFraction;
        _shieldBar.fillAmount = sFraction;
    }
    private void OnDisable()
    {
        DOTween.KillAll();
    }
}
