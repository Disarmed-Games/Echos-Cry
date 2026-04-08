using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class FirePitHandler : MonoBehaviour
{
    [SerializeField] private float _damage = 5f;
    [SerializeField] private GameObject projectileObject;
    [SerializeField] private float _force;
    [SerializeField] private Vector2 randomSpawnInterval;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.Execute(_damage);
        }
    }

    private void Start()
    {
        StartCoroutine(ProjectileLoop());
    }

    IEnumerator ProjectileLoop()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {
            FireProjectile();

            float delay = Random.Range(randomSpawnInterval.x, randomSpawnInterval.y);
            yield return new WaitForSeconds(delay);
        }
    }

    Vector3 RandomHemisphereDirection(float minAngle, float maxAngle)
    {
        float minRad = minAngle * Mathf.Deg2Rad;
        float maxRad = maxAngle * Mathf.Deg2Rad;

        float theta = Random.Range(minRad, maxRad);

        float phi = Random.Range(0f, Mathf.PI * 2f);

        float x = Mathf.Sin(theta) * Mathf.Cos(phi);
        float y = Mathf.Cos(theta);
        float z = Mathf.Sin(theta) * Mathf.Sin(phi);

        return new Vector3(x, y, z).normalized;
    }

    void FireProjectile()
    {
        Vector3 direction = RandomHemisphereDirection(30f, 70f);
        GameObject projectileInstance = Instantiate(projectileObject, transform.position + new Vector3(0, 0.2f, 0), Quaternion.identity);
        Rigidbody projectileRB = projectileInstance.GetComponent<Rigidbody>();
        projectileRB.AddForce(direction * _force, ForceMode.Impulse);
    }
}
