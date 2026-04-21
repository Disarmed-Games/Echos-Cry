using TMPro;
using UnityEngine;

public class HitQualityTextUI : MonoBehaviour
{
    static private int lightFlashHash = Animator.StringToHash("LightFlash");
    [SerializeField] private GameObject _hitQualityObject;
    [SerializeField] private Animator _hitEffectAnimator1;
    [SerializeField] private Animator _hitEffectAnimator2;
    [SerializeField] private bool showHitText = true;

    public void UpdateText()
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

        _hitEffectAnimator1.Play(lightFlashHash);
        _hitEffectAnimator2.Play(lightFlashHash); 
    }
}
