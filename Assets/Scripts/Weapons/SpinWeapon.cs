using UnityEngine;

public class SpinWeapon : WeaponBase
{
    [SerializeField] private float spinDuration = 0.5f;
    [SerializeField] private float minSpinDuration = 0.1f;

    private bool isSpinning = false;
    private float spinEndTime;
    private PlayerMovement playerMovement;
    private float currentRotation = 0f;

    protected override void Awake()
    {
        base.Awake();
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    protected override void Attack()
    {
        if (!isSpinning)
        {
            StartSpin();
            DealAOEDamage();
        }
    }

    private void StartSpin()
    {
        isSpinning = true;
        currentRotation = 0f;
        spinEndTime = Time.time + CalculateDuration();
        playerMovement.SetRotationLocked(true); // Disable mouse rotation
    }

    private float CalculateDuration()
    {
        var duration = spinDuration * GetModifiedProjectileSpeed();
        duration = Mathf.Max(duration, minSpinDuration);
        return duration;
    }

    protected override void Update()
    {
        if (Time.time > nextAttackTime && !isSpinning)
        {
            Attack();
            nextAttackTime = Time.time + Mathf.Max(GetModifiedCooldown(), spinDuration);
        }

        if (isSpinning)
        {
            float rotationProgress = Time.deltaTime / (spinEndTime - Time.time + Time.deltaTime);
            float rotationThisFrame = Mathf.Min(360f - currentRotation, 360f * rotationProgress);

            //player.transform.Rotate(Vector3.up, rotationThisFrame);
            currentRotation += rotationThisFrame;
            if (currentRotation >= 360f || Time.time >= spinEndTime)
            {
                isSpinning = false;
                playerMovement.SetRotationLocked(false);
            }
        }
    }

    private void DealAOEDamage()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, GetModifiedArea());

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out IDamagable enemy))
            {
                enemy.Damage((int)GetModifiedDamage());
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        float areaToShow = baseStats.baseArea;

        // If we have a player reference, modify the area based on stats
        PlayerScript playerRef = GetComponentInParent<PlayerScript>();
        if (playerRef != null)
        {
            PlayerScript.Stats stats = playerRef.GetCurrentStats();
            if (stats != null)
            {
                areaToShow *= (stats.areaPercent / 100f);
            }
        }

        Gizmos.DrawWireSphere(transform.position, areaToShow);
    }
}
