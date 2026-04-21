using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] HitQualityTextUI _hitQualityText;

    public HitQualityTextUI HitQualityText { get => _hitQualityText; }
}
