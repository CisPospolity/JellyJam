using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [System.Serializable]
    public class WeaponStats
    {
        [SerializeField] public float baseDamage = 1f;
        [SerializeField] public float baseCooldown = 1f;
        [SerializeField] public float baseArea = 1f;
        [SerializeField] public float baseProjectileSpeed = 1f;
        [SerializeField] public int baseProjectileCount = 1;
        [SerializeField] public float baseDuration = 1f;
    }

    [SerializeField] protected WeaponStats baseStats = new WeaponStats();
    protected PlayerScript player;
    protected PlayerScript.Stats cachedStats = new PlayerScript.Stats();
    protected float nextAttackTime;

    protected virtual void Awake()
    {
        player = GetComponentInParent<PlayerScript>();
        player.OnStatsChanged += UpdateCachedStats;
        UpdateCachedStats();
    }
    private void UpdateCachedStats()
    {
        cachedStats = player.GetCurrentStats();
    }

    protected virtual void Update()
    {
        if(Time.time > nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + GetModifiedCooldown();
        }
    }

    protected abstract void Attack();

    protected float GetModifiedDamage()
    {
        return baseStats.baseDamage * (cachedStats.damagePercent / 100f);
    }

    protected float GetModifiedArea()
    {
        return baseStats.baseArea * (cachedStats.areaPercent / 100f);
    }

    protected float GetModifiedCooldown()
    {
        return baseStats.baseCooldown * (cachedStats.cooldownPercent / 100f);
    }

    protected float GetModifiedProjectileSpeed()
    {
        return baseStats.baseProjectileSpeed * (cachedStats.projectileSpeedPercent / 100f);
    }

    protected int GetTotalProjectileCount()
    {
        return baseStats.baseProjectileCount + cachedStats.additionalProjectileCount;
    }

    protected float GetModifiedDuration()
    {
        return baseStats.baseDuration * (cachedStats.durationPercent / 100f);
    }

    private void OnDestroy()
    {
        player.OnStatsChanged -= UpdateCachedStats;

    }
}
