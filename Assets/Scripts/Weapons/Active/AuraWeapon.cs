using UnityEngine;
using UnityEngine.VFX;

public class AuraWeapon : WeaponBase
{
    [SerializeField] private VisualEffect vfxGraph;
    private GameObject vfxHolder;

    protected override void Awake()
    {
        base.Awake();

        vfxHolder = new GameObject("Aura_VFX_Holder");
        vfxHolder.transform.position = transform.position;

        vfxGraph.transform.parent = vfxHolder.transform;
    }

    protected override void Update()
    {
        base.Update();
        if (vfxHolder != null)
        {
            vfxHolder.transform.position = transform.position;
        }
    }
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

        if(vfxGraph != null)
        {
            vfxGraph.SetFloat("Radius", GetModifiedArea());
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
