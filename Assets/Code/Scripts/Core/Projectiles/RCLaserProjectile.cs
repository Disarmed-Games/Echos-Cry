using Codice.Client.Common.GameUI;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using static UnityEngine.UI.Image;

public class RCLaserProjectile : AttackMethod
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float maxRayLength;
    [SerializeField] private float raySpeed;
    [SerializeField] private float targetLerpSpeed = 0.5f;
    [SerializeField] private float _duration;
    [SerializeField] private GameObject laserPrefab;

    private GameObject currentLaser;
    private Vector3 smoothedTarget;
    private float rayLength;
    private float projectileDamage;

    public override void Execute(float damage, Vector3 direction, Transform origin)
    {
        projectileDamage = damage;
        _attackEnded = false;
        currentLaser = Instantiate(laserPrefab, transform);
        smoothedTarget = PlayerRef.Transform.position;

        StartCoroutine(StartAttack());
        StartCoroutine(AttackDuration(_duration));
    }

    private IEnumerator StartAttack()
    {
        while (true)
        {
            rayLength += raySpeed * Time.deltaTime;
            rayLength = Mathf.Min(rayLength, maxRayLength);

            smoothedTarget = Vector3.Lerp(
                smoothedTarget,
                PlayerRef.Transform.position,
                targetLerpSpeed * Time.deltaTime
            );

            Vector3 direction = (smoothedTarget - transform.position).normalized;
            direction.y = 0f;

            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            currentLaser.transform.localPosition = Vector3.forward * (rayLength * 0.5f);
            currentLaser.transform.localScale = new Vector3(rayLength, 0.5f, 1.0f);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, rayLength, layerMask) 
                && !Player.Instance.Health.IsInvincible)
            {
                Damage(hit.collider);
                AttackFinished();
                break;
            }
            else yield return null;
        } 
    }

    private IEnumerator AttackDuration(float dur)
    {
        yield return new WaitForSeconds(dur);
        AttackFinished();
    }

    private void AttackFinished()
    {
        rayLength = 0;
        _attackEnded = true;
        Destroy(currentLaser);
    }

    private void Damage(Collider collider)
    {
        AttackInfo attack = new AttackInfo.Builder().SetDamage(projectileDamage).Build();
        collider.GetComponent<IDamageable>().Execute(attack);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(Vector3.forward) * rayLength);
    }
}
