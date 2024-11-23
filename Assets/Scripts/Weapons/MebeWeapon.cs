using System.Collections;
using UnityEngine;

public class MebeWeapon : WeaponBase
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private float maxTargetDistance = 10f;
    [SerializeField] private float projectileDelayBetween = 0.1f;
    private Transform target;

    [Header("VFX")]
    [SerializeField] private GameObject mebePrefab;
    [SerializeField] private Vector3 relativeOffset = new Vector3(1, 1.75f, -1f);
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float maxMoveSpeed = 10f;
    [SerializeField] private float floatAmplitude = 0.2f;
    [SerializeField] private float floatSpeed = 2f;

    private Transform mebe;
    private Vector3 initialOffset;
    private float startTime;
    private Vector3 currentVelocity;

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

    private void Start()
    {
        startTime = Time.time;
        mebe = Instantiate(mebePrefab, player.transform.position, Quaternion.identity, null).transform;
        initialOffset = relativeOffset;
        startTime = Time.time;

        mebe.position = GetTargetPosition();
    }

    private Vector3 GetTargetPosition(Vector3 offset = default)
    {
        if (offset == default) offset = relativeOffset;
        return player.transform.position + player.transform.TransformDirection(offset);
    }

    protected override void Update()
    {
        base.Update();

        if (mebe == null) return;
        float floatOffset = Mathf.Sin((Time.time - startTime) * floatSpeed) * floatAmplitude;

        Vector3 currentOffset = initialOffset + Vector3.up * floatOffset;

        // Convert local offset to world space based on target's rotation
        Vector3 targetPosition = GetTargetPosition(currentOffset);

        // Smooth damp for position
        mebe.position = Vector3.SmoothDamp(
            mebe.position,
            targetPosition,
            ref currentVelocity,
            1f / smoothSpeed,
            maxMoveSpeed
        );
    }

    private void SpawnProjectile()
    {
        target = FindNearestEnemy();
        if (target == null) return;

        var spawnedProjectile = Instantiate(projectile, mebe.transform.position, Quaternion.identity);
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

    private void OnDestroy()
    {
        if (mebe != null)
        {
            Destroy(mebe.gameObject);
        }
    }

    private void OnEnable()
    {
        if(mebe != null)
        {
            mebe.gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        if (mebe != null)
        {
            mebe.gameObject.SetActive(false);
        }
    }
}
