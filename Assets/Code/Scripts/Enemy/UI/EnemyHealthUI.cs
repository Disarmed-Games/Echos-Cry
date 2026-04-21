using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    [SerializeField] private Image _healthBarFront;
    [SerializeField] private Image _healthBarBack;
    [SerializeField] private Image _shieldBar;
    [SerializeField] private float frontSpeed = 8f;
    [SerializeField] private float backSpeed = 3f;

    private float targetHealthFraction;

    private void Update()
    {
        if (_healthBarBack.fillAmount > targetHealthFraction)
        { //Damage
            _healthBarFront.fillAmount = targetHealthFraction;

            _healthBarBack.fillAmount = Mathf.Lerp(
                _healthBarBack.fillAmount,
                targetHealthFraction,
                backSpeed * Time.deltaTime
            );
        }
        else if (_healthBarFront.fillAmount < targetHealthFraction)
        { //Healing
            _healthBarBack.fillAmount = targetHealthFraction;

            _healthBarFront.fillAmount = Mathf.Lerp(
                _healthBarFront.fillAmount,
                targetHealthFraction,
                frontSpeed * Time.deltaTime
            );
        }
    }

    public void UpdateUI(float currentHealth, float maxHealth, float currentShield, float maxShield)
    {
        targetHealthFraction = currentHealth / maxHealth;

        float shieldFraction = (maxShield > 0) ? currentShield / maxShield : 0f;
        _shieldBar.fillAmount = shieldFraction;
    }

    public void ResetUI(float currentHealth, float maxHealth, float currentShield, float maxShield)
    {
        float hFraction = (maxHealth > 0) ? currentHealth / maxHealth : 1f;
        float sFraction = (maxShield > 0) ? currentShield / maxShield : 0f;

        targetHealthFraction = hFraction;

        _healthBarFront.fillAmount = hFraction;
        _healthBarBack.fillAmount = hFraction;
        _shieldBar.fillAmount = sFraction;
    }
}