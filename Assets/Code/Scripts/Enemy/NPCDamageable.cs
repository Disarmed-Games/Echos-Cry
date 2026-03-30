using UnityEngine;

public class NPCDamageable : MonoBehaviour, IDamageable
{
    [SerializeField] private Enemy _npc;
    [SerializeField] private EnemyStateCache.EnemyStates _staggerState;
    private bool _armorBreak = false;

    public virtual void Execute(float amount)
    {
        _npc.Collider.enabled = false;

        amount *= _npc.Health.DamageMultiplier;

        _npc.Health.Damage(amount);
        if(_npc.Health.CurrentArmor > 0)
        {
            if(GlobalSFXManager.Instance != null && GlobalSFXManager.Instance.ArmorHitSFX) 
                _npc.SoundStrategy.Execute(GlobalSFXManager.Instance.ArmorHitSFX, _npc.transform, 0);
            _npc.NPCAnimator.TintFlash(Color.blue, 0.2f);
        }
        else
        {
            if (!_armorBreak)
            {
                _armorBreak = true;
                if (GlobalSFXManager.Instance != null && GlobalSFXManager.Instance.ArmorBreakSFX)
                    _npc.SoundStrategy.Execute(GlobalSFXManager.Instance.ArmorBreakSFX, _npc.transform, 0);
            }
            _npc.SoundStrategy.Execute(_npc.SoundConfig.HitSFX, _npc.transform, 0);
            _npc.NPCAnimator.TintFlash(Color.red, 0.2f);
            _npc.NPCAnimator.PlayVisualEffect();
        }
            
        if(DamageLabelManager.Instance != null)
            DamageLabelManager.Instance.SpawnPopup(amount, _npc.transform.position, Color.white);
        
        if(_npc.Health.CurrentArmor <= 0)
        {
            //_npc.StateMachine.SwitchState(_npc.StateCache.RequestState(_staggerState));
            _npc.StateData.IsStaggered = true;
        }
    }
}

public struct AttackInfo
{
    public float damage;
    public float force;
    public ForceMode forceMode;
    public TempoConductor.HitQuality hitQuality;
    public Vector3 direction;
}
