using Ink.Parsed;
using UnityEngine;

public class AttackInfo
{
    public float Damage { get; private set; }
    public float Force { get; private set; }
    public ForceMode ForceMode { get; private set; }
    public TempoConductor.HitQuality HitQuality { get; private set; }
    public Vector3 Direction { get; private set; }
    public Transform Origin { get; private set; }
    public EffectData[] Effects { get; private set; }

    public class Builder
    {
        private float damage = 10f;
        private float force = 5f;
        private ForceMode forceMode = ForceMode.Force;
        private TempoConductor.HitQuality hitQuality = TempoConductor.HitQuality.Miss;
        private Vector3 direction = Vector3.zero;
        private Transform origin = null;
        private EffectData[] effects = null;

        public Builder SetDamage(float amount)
        {
            this.damage = amount; 
            return this;
        }
        public Builder SetForce(float amount)
        {
            this.force = amount;
            return this;
        }
        public Builder SetForceMode(ForceMode forceMode)
        {
            this.forceMode = forceMode;
            return this;
        }
        public Builder SetHitQuality(TempoConductor.HitQuality hitQuality)
        {
            this.hitQuality = hitQuality;
            return this;
        }
        public Builder SetDirection(Vector3 direction)
        {
            this.direction = direction;
            return this;
        }
        public Builder SetOrigin(Transform transform)
        {
            this.origin = transform;
            return this;
        }
        public Builder SetEffects(EffectData[] effects)
        {
            this.effects = effects;
            return this;
        }
        public AttackInfo Build()
        {
            AttackInfo attack = new()
            {
                Damage = damage,
                Force = force,
                ForceMode = forceMode,
                HitQuality = hitQuality,
                Direction = direction,
                Origin = origin,
                Effects = effects
            };
            return attack;
        }
    }
}
