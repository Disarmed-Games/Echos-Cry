using Mono.Cecil.Cil;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class BeatUIHandler : MonoBehaviour
{
    [SerializeField] private InputTranslator _translator;
    [SerializeField] private TextMeshProUGUI hitQualityText;
    [SerializeField] private Animator _textAnimator;
    [SerializeField] private Animator _hitEffectAnimator1;
    [SerializeField] private Animator _hitEffectAnimator2;
    [SerializeField] private Image heavyAttackImage;
    [SerializeField] private Sprite[] heavyAttackSprites = new Sprite[3];
    [SerializeField] private bool showHitText = true;

    void OnEnable()
    {
        //_translator.OnDashEvent += UpdateHitQualityText;
        _translator.OnPrimaryActionEvent += UpdateHitQualityText;
        _translator.OnSecondaryActionEvent += UpdateHitQualityText;
    }

    private void OnDisable()
    {
        //_translator.OnDashEvent -= UpdateHitQualityText;
        _translator.OnPrimaryActionEvent -= UpdateHitQualityText;
        _translator.OnSecondaryActionEvent -= UpdateHitQualityText;
    }

    private void Update()
    {
        if (showHitText)
        {
            heavyAttackImage.enabled = true;
            if (BeatManager.Instance.BeatInMeasure == 0)
            {
                heavyAttackImage.sprite = heavyAttackSprites[0];
                heavyAttackImage.material.SetTexture("_Texture2D", heavyAttackSprites[0].texture);
            }
            else if (BeatManager.Instance.BeatInMeasure == 1)
            {
                heavyAttackImage.sprite = heavyAttackSprites[1];
                heavyAttackImage.material.SetTexture("_Texture2D", heavyAttackSprites[1].texture);
            }
            else if (BeatManager.Instance.BeatInMeasure == 2)
            {
                heavyAttackImage.sprite = heavyAttackSprites[0];
                heavyAttackImage.material.SetTexture("_Texture2D", heavyAttackSprites[0].texture);
            }
            else
            {
                heavyAttackImage.sprite = heavyAttackSprites[2];
                heavyAttackImage.material.SetTexture("_Texture2D", heavyAttackSprites[2].texture);
            }
        }
        else heavyAttackImage.enabled = false;
    }

    private void UpdateHitQualityText(bool isPressed)
    {
        if (!isPressed) return;

        //On Beat Text
        switch (TempoConductor.Instance.CurrentHitQuality)
        {
            case TempoConductor.HitQuality.Excellent:
                hitQualityText.color = new Color(110f / 255f, 44f / 255f, 222f / 255f, 1f); //purple
                break;
            case TempoConductor.HitQuality.Good:
                hitQualityText.color = new Color(47f / 255f, 235f / 255f, 81f / 255f, 1.0f);
                break;
            case TempoConductor.HitQuality.Miss:
                hitQualityText.color = Color.red;
                break;
        }

        if (showHitText)
        {
            hitQualityText.text = TempoConductor.Instance.CurrentHitQuality.ToString();
            _textAnimator.SetTrigger("Bounce");
        }

        _hitEffectAnimator1.SetTrigger("Effect");
        _hitEffectAnimator2.SetTrigger("Effect");
    }
}
