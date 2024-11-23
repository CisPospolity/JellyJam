using UnityEngine;

public class AuraWeapon : WeaponBase
{
    protected override void Attack()
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
