using UnityEngine;

public class EnemyDamageable : MonoBehaviour, IDamageable
{
    [SerializeField] private Enemy _enemy;
    private bool _armorBreak = false;

    public virtual void Execute(float amount)
    {
        _enemy.Collider.enabled = false;

        amount *= _enemy.Health.DamageMultiplier;

        _enemy.Health.Damage(amount);
        if(_enemy.Health.CurrentArmor > 0)
        {
            if(GlobalSFXManager.Instance != null && GlobalSFXManager.Instance.ArmorHitSFX) 
                _enemy.SoundStrategy.Execute(GlobalSFXManager.Instance.ArmorHitSFX, _enemy.transform, 0);
            _enemy.NPCAnimator.TintFlash(Color.blue, 0.2f);
        }
        else
        {
            if (!_armorBreak)
            {
                _armorBreak = true;
                if (GlobalSFXManager.Instance != null && GlobalSFXManager.Instance.ArmorBreakSFX)
                    _enemy.SoundStrategy.Execute(GlobalSFXManager.Instance.ArmorBreakSFX, _enemy.transform, 0);
            }
            DecalManager.Instance.GetBloodDecal().transform.position = _enemy.transform.position;
            _enemy.SoundStrategy.Execute(_enemy.SoundConfig.HitSFX, _enemy.transform, 0);
            _enemy.NPCAnimator.TintFlash(Color.red, 0.2f);
            _enemy.NPCAnimator.PlayVisualEffect();
        }
            
        if(DamageLabelManager.Instance != null)
            DamageLabelManager.Instance.SpawnPopup(amount, _enemy.transform.position, Color.white);
        
        if(_enemy.Health.CurrentArmor <= 0) _enemy.StateData.IsStaggered = true;
        
        if(_enemy.EnemyHealthUI != null) _enemy.EnemyHealthUI.UpdateUI(_enemy.Health.CurrentHealth, 
            _enemy.Health.MaxHealth, 
            _enemy.Health.CurrentArmor, 
            _enemy.Health.MaxArmor);
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
