using UnityEngine;
using EchosCry.Combo;

public class TestingEffects : MonoBehaviour
{
    [SerializeField] private EventChannel channel;

    [SerializeField] private EffectData flame;
    [SerializeField] private EffectData bleed;
    [SerializeField] private EffectData mark;

    private bool added = false;
    [SerializeField] private bool adding = false;

    private void Update()
    {
        if(adding && !added)
        {
            Player.Instance.WeaponHolder.AddEffectPrimary(StateName.Light1, flame);
            Player.Instance.WeaponHolder.AddEffectPrimary(StateName.Light2, flame);
            Player.Instance.WeaponHolder.AddEffectPrimary(StateName.Light3, flame);
            Player.Instance.WeaponHolder.AddEffectPrimary(StateName.Light4, flame);
            Player.Instance.WeaponHolder.AddEffectPrimary(StateName.Light5, flame);

            Player.Instance.WeaponHolder.AddEffectPrimary(StateName.Light1, bleed);
            Player.Instance.WeaponHolder.AddEffectPrimary(StateName.Light2, bleed);
            Player.Instance.WeaponHolder.AddEffectPrimary(StateName.Light3, bleed);
            Player.Instance.WeaponHolder.AddEffectPrimary(StateName.Light4, bleed);
            Player.Instance.WeaponHolder.AddEffectPrimary(StateName.Light5, bleed);
            added = true;
        }
    }
}
