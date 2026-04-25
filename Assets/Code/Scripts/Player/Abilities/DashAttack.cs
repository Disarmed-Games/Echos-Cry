using UnityEngine;

public class DashAttack : MonoBehaviour
{
    [SerializeField] DashAttackData dashAttackData;
    public TempoConductor.HitQuality HitQuality;

    private void OnDisable()
    {
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        AttackInfo attack = new AttackInfo.Builder()
            .SetDamage(dashAttackData.Damage)
            .SetHitQuality(HitQuality)
            .SetOrigin(Player.Instance.transform)
            .Build();
        other.GetComponent<IDamageable>()?.Execute(attack);
    }
}