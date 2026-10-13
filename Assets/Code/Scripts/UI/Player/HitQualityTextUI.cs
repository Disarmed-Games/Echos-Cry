using TMPro;
using UnityEngine;

public class HitQualityTextUI : MonoBehaviour
{
    private static readonly int lightFlashHash = Animator.StringToHash("LightFlash");
    [SerializeField] private GameObject _hitQualityObject;
    [SerializeField] private Animator _hitEffectAnimator1;
    [SerializeField] private Animator _hitEffectAnimator2;
    private GameObject activeHitText;

    public void UpdateText()
    {
        UpdateText(TempoConductor.Instance.CurrentHitQuality);
    }

    public void UpdateText(TempoConductor.HitQuality quality)
    {
        // Replace the previous result immediately so two animated labels cannot overlap.
        if (activeHitText != null)
        {
            activeHitText.SetActive(false);
            Destroy(activeHitText);
        }
        GameObject hitTextObject = Instantiate(_hitQualityObject, transform);
        activeHitText = hitTextObject;
        hitTextObject.GetComponent<TextMeshProUGUI>().text = quality.ToString();
        hitTextObject.GetComponent<Animator>().SetTrigger("Bounce");

        switch (quality)
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

        if (_hitEffectAnimator1 != null) _hitEffectAnimator1.Play(lightFlashHash);
        if (_hitEffectAnimator2 != null) _hitEffectAnimator2.Play(lightFlashHash);
    }
}
