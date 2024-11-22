using System.Collections;
using UnityEngine;

public class TestWeapon : WeaponBase
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private float maxTargetDistance = 10f;
    [SerializeField] private float projectileDelayBetween = 0.1f;
    private Transform target;

    private IEnumerator FireProjectiles(int count)
    {
        for(int i = 0; i < count; i++)
        {
            SpawnProjectile();
            if(i<count-1)
            {
                yield return new WaitForSeconds(projectileDelayBetween);
            }
        }
    }

    private void SpawnProjectile()
    {
        target = FindNearestEnemy();
        if (target == null) return;

        var spawnedProjectile = Instantiate(projectile, transform.position, Quaternion.identity);
        TestProjectile proj = spawnedProjectile.GetComponent<TestProjectile>();
        Vector3 newPos = new Vector3(target.position.x, transform.position.y, target.position.z);
        if(proj != null)
        {
            proj.Initialize(
                newPos,
                GetModifiedDamage(),
                GetModifiedProjectileSpeed(),
                GetModifiedArea());
        }
    }
    protected override void Attack()
    {
        int count = GetTotalProjectileCount();
        if (count <= 0) return;

        StartCoroutine(FireProjectiles(count));
    }

    private Transform FindNearestEnemy()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, maxTargetDistance);
        Transform nearestEnemy = null;
        float nearestDistance = maxTargetDistance;

        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = collider.transform;
                }
            }
        }

        return nearestEnemy;
    }
}
