using AudioSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HeatGaugeUI : MonoBehaviour
{
    [SerializeField] DoubleIntEventChannel _updateHeatGauge;
    [SerializeField] Sprite defaultHeatImage;
    [SerializeField] Sprite maxHeatImage;
    [SerializeField] Sprite disabledHeatImage;
    [SerializeField] private GameObject heatIconPrefab;
    [SerializeField] private List<GameObject> heatGuages;
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
    public void AddHeatGuage()
    {
        GameObject newHeatIcon = Instantiate(heatIconPrefab, transform);
        heatGuages.Add(newHeatIcon);
    }
    private void UpdateHeatGauge(int current, int max)
    {
        for (int i = 0; i < max; i++)
        {
            if (current == max)
                heatGuages[i].GetComponent<Image>().sprite = maxHeatImage;
            else if (i < current)
                heatGuages[i].GetComponent<Image>().sprite = defaultHeatImage;
            else
                heatGuages[i].GetComponent<Image>().sprite = disabledHeatImage;
        }

        int index = Mathf.Clamp(current - 1, 0, heatGuages.Count() - 1);
        heatGuages[index].GetComponent<Animator>().SetTrigger("Spin");

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
