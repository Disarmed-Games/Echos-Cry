using TMPro;
using UnityEngine;

public class BeatUIHandler : MonoBehaviour
{
    [SerializeField] private InputTranslator _translator;
    [SerializeField] private GameObject _hitQualityObject;
    [SerializeField] private Animator _hitEffectAnimator1;
    [SerializeField] private Animator _hitEffectAnimator2;
    [SerializeField] private bool showHitText = true;

    void OnEnable()
    {
        //_translator.OnDashEvent += UpdateHitQualityText;
        _translator.OnPrimaryActionEvent += UpdateHitQualityText;
        _translator.OnSecondaryActionEvent += UpdateHitQualityText;
        _translator.OnSpecialAttackEvent += UpdateHitQualityText;
    }

    private void OnDisable()
    {
        //_translator.OnDashEvent -= UpdateHitQualityText;
        _translator.OnPrimaryActionEvent -= UpdateHitQualityText;
        _translator.OnSecondaryActionEvent -= UpdateHitQualityText;
        _translator.OnSpecialAttackEvent -= UpdateHitQualityText;
    }

    private void UpdateHitQualityText(bool isPressed)
    {
        if (!isPressed) return;

        //On Beat Text
        if (showHitText)
        {
            GameObject hitTextObject = Instantiate(_hitQualityObject, transform);
            hitTextObject.GetComponent<TextMeshProUGUI>().text = TempoConductor.Instance.CurrentHitQuality.ToString();
            hitTextObject.GetComponent<Animator>().SetTrigger("Bounce");

            switch (TempoConductor.Instance.CurrentHitQuality)
            {
                case TempoConductor.HitQuality.Excellent:
                    hitTextObject.GetComponent<TextMeshProUGUI>().color = new Color(110f / 255f, 44f / 255f, 222f / 255f, 1f); //purple
                    break;
                case TempoConductor.HitQuality.Good:
                    hitTextObject.GetComponent<TextMeshProUGUI>().color = new Color(47f / 255f, 235f / 255f, 81f / 255f, 1.0f);
                    break;
                case TempoConductor.HitQuality.Miss:
                    hitTextObject.GetComponent<TextMeshProUGUI>().color = Color.red;
                    break;
            }
        }

        _hitEffectAnimator1.SetTrigger("Effect");
        _hitEffectAnimator2.SetTrigger("Effect");
    }
}
