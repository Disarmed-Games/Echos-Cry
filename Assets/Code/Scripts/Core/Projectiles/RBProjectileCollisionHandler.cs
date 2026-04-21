using System;
using System.Collections;
using UnityEngine;

public class RBPRojectileCollisionHandler : MonoBehaviour
{
    private RBProjectilePool handler;
    private Rigidbody rb;

    private float projectileDamage = 0;
    [SerializeField] private float timer = 5;
   
    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.layer == LayerMask.NameToLayer("PlayerMeleeAttack")
        //    && currentUser == ProjectileUser.ENEMY)
        //{
        //    StopAllCoroutines();
        //    StartCoroutine(WaitForTime());

        //    //Switch to attacking enemies
        //    ParryProjectile();

        //    return;
        //}

        ////Only damage if colliding with valid target
        //if ((currentUser == ProjectileUser.ENEMY && other.gameObject.layer != LayerMask.NameToLayer("Player"))
        //    || (currentUser == ProjectileUser.PLAYER && other.gameObject.layer != LayerMask.NameToLayer("Enemy")))
        //{
        //    return;
        //}

        //damageEnemyAction(other);
        Damage(other);
        StopAllCoroutines();
        ReleaseProjectile();
    }
    private void Damage(Collider collider)
    {
        AttackInfo attack = new AttackInfo.Builder().SetDamage(projectileDamage).Build();
        collider.GetComponent<IDamageable>().Execute(attack);
    }

    IEnumerator WaitForTime()
    {
        yield return new WaitForSeconds(timer);
        ReleaseProjectile();
    }
    public void ReleaseProjectile()
    {
        if (handler != null) handler.ReleaseProjectile(rb);
    }
    public void SetHandler(RBProjectilePool handler)
    {
        if (this.handler != null) return;
        this.handler = handler;
    }
    public void SetDamage(float damage)
    {
        projectileDamage = damage;
    }

    //private void ParryProjectile()
    //{
    //    //currentUser = ProjectileUser.PLAYER;

    //    if (rb == null) return;
    //    Vector3 projectileDirection = PlayerOrientation.Direction;
    //    rb.linearVelocity = projectileDirection * rb.linearVelocity.magnitude;
    //}

    private void Awake()
    {
        handler = GetComponentInParent<RBProjectilePool>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        //currentUser = startingUser;
        StartCoroutine(WaitForTime());
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
