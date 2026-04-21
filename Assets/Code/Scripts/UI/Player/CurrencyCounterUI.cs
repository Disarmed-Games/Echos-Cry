using TMPro;
using UnityEngine;
using DG.Tweening;
public class CurrencyCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private Transform currencyTransform;
    private float transitionTime = 0.15f;

    public void UpdateCurrencyText(int currencyCount)
    {
        currencyTransform.DOKill();
        currencyTransform.localRotation = Quaternion.identity;
        currencyTransform.DOShakeRotation(transitionTime, 20, 5, 45, true);
        currencyText.text = "Gold: " + currencyCount.ToString();
    }

    private void Start()
    {
        PlayerCurrencySystem.OnCurrencyChangeEvent += UpdateCurrencyText;
    }
    private void OnDestroy()
    {
       PlayerCurrencySystem.OnCurrencyChangeEvent -= UpdateCurrencyText;
    }
}
