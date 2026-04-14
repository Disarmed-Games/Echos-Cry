using UnityEngine;
using UnityEngine.UI;
using AudioSystem;

public class HeatGaugeUI : MonoBehaviour
{
    [SerializeField] DoubleIntEventChannel _updateHeatGauge;
    [SerializeField] Image[] heatImagesArray = new Image[6];
    [SerializeField] private soundEffect useGaugeSFX;
    private int pastGuageValue;

    private void OnEnable()
    {
        if (_updateHeatGauge != null) _updateHeatGauge.Channel += UpdateHeatGauge;
        UpdateHeatGauge(0, 6);
    }
    private void OnDisable()
    {
        if (_updateHeatGauge != null) _updateHeatGauge.Channel -= UpdateHeatGauge;
    }

    private void UpdateHeatGauge(int current, int max)
    {
        for (int i = 0; i < max; i++)
        {
            heatImagesArray[i].enabled = (i < current);
        }

        if (current < pastGuageValue)
        {
            SoundEffectManager.Instance.Builder
            .SetSound(useGaugeSFX)
            .SetSoundPosition(transform.position)
            .ValidateAndPlaySound();
        }

        if (current != pastGuageValue) pastGuageValue = current;
    }
}
