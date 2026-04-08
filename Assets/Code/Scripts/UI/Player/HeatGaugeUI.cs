using TMPro;
using UnityEngine;

public class HeatGaugeUI : MonoBehaviour
{
    [SerializeField] DoubleIntEventChannel _updateHeatGauge;
    [SerializeField] TextMeshProUGUI text;

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
        text.text = current.ToString() + " / " + max.ToString();
    }
}
