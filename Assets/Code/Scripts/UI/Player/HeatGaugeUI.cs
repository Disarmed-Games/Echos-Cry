using UnityEngine;
using UnityEngine.UI;

public class HeatGaugeUI : MonoBehaviour
{
    [SerializeField] DoubleIntEventChannel _updateHeatGauge;
    [SerializeField] Image[] heatImagesArray = new Image[6];
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
    }
}
