using System;
using UnityEngine;
using UnityEngine.UI;

public class AttackIndicatorUI : MonoBehaviour
{

[Serializable]
struct SpriteNode
{
    public Sprite sprite;
    public Color color;
};

    [SerializeField] private Image heavyAttackImage;
    [SerializeField] private SpriteNode[] spriteNodes = new SpriteNode[3];

    private void OnEnable()
    {
        heavyAttackImage.enabled = true;
    }
    private void OnDisable()
    {
        heavyAttackImage.enabled = false;
    }

    private void SetAttackIndicator(int index)
    {
        heavyAttackImage.sprite = spriteNodes[index].sprite;
        heavyAttackImage.material.SetTexture("_Texture2D", spriteNodes[index].sprite.texture);
        heavyAttackImage.color = spriteNodes[index].color;
    }

    void Update()
    {
        float currentProgress = BeatManager.Instance.BeatInMeasure + BeatManager.Instance.BeatProgress;

        float goodPercent = TempoConductor.Instance.GoodPercent;
        float targetProgressLow1 = 1 - goodPercent;
        float targetProgressHigh1 = goodPercent + 1;

        float targetProgressLow2 = 3 - goodPercent;
        float targetProgressHigh2 = goodPercent + 3;

        if (currentProgress >= targetProgressLow1 
            && currentProgress <= targetProgressHigh1
            && Player.Instance.HeatGauge.CurrentCharge >= 6)
        {
            SetAttackIndicator(1);
        }
        else if (currentProgress >= targetProgressLow2
            && currentProgress <= targetProgressHigh2
            && Player.Instance.HeatGauge.CurrentCharge >= 6)
        {
            SetAttackIndicator(2);
        }
        else
        {
            SetAttackIndicator(0);
        }
    }
}
