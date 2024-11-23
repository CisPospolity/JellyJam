using UnityEngine;

public class PassiveWeaponBase : WeaponBase
{
    protected PlayerScript.Stats appliedStats = new PlayerScript.Stats();

    protected virtual void OnEnable()
    {
        weaponType = WeaponType.Passive;
        
    }

    protected virtual void AddPassiveStats()
    {
        appliedStats = new PlayerScript.Stats();
        CalculatePassiveStats();
        player.ModifyStats(appliedStats);
    }

    protected virtual void RemovePassiveStats()
    {
        PlayerScript.Stats negativeStats = new PlayerScript.Stats();
        // Negate all the stats
        negativeStats.damagePercent = -appliedStats.damagePercent;
        negativeStats.areaPercent = -appliedStats.areaPercent;
        negativeStats.cooldownPercent = -appliedStats.cooldownPercent;
        negativeStats.projectileSpeedPercent = -appliedStats.projectileSpeedPercent;
        negativeStats.additionalProjectileCount = -appliedStats.additionalProjectileCount;
        negativeStats.durationPercent = -appliedStats.durationPercent;

        player.ModifyStats(negativeStats);
    }

    protected virtual void CalculatePassiveStats()
    {
        appliedStats.damagePercent = baseStats.baseDamage;
        appliedStats.areaPercent = baseStats.baseArea;
        appliedStats.cooldownPercent = baseStats.baseCooldown;
        appliedStats.projectileSpeedPercent = baseStats.baseProjectileSpeed;
        appliedStats.additionalProjectileCount = baseStats.baseProjectileCount;
        appliedStats.durationPercent = baseStats.baseDuration;
    }

    public override void LevelUp()
    {
        base.LevelUp();
        RemovePassiveStats();
        AddPassiveStats();
    }
}
