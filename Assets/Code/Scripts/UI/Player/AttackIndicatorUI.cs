using System;
using UnityEngine;
using UnityEngine.UI;
using static TempoConductor;

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
        if (Player.Instance.HeatGauge.CurrentCharge < 6)
        {
            SetAttackIndicator(0);
            return;
        }

        float progress = BeatManager.Instance.BeatProgress;
        float goodPercent = TempoConductor.Instance.GoodPercent;

        if (BeatManager.Instance.BeatInMeasure == 0 && progress >= (1f - goodPercent)
            || BeatManager.Instance.BeatInMeasure == 1 && progress <= goodPercent * 0.5f)
            SetAttackIndicator(1);
        else if (BeatManager.Instance.BeatInMeasure == 2 && progress >= (1f - goodPercent)
            || BeatManager.Instance.BeatInMeasure == 3 && progress <= goodPercent * 0.5f)
            SetAttackIndicator(2);
        else
            SetAttackIndicator(0);
    }
}
