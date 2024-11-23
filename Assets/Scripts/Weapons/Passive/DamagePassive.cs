using UnityEngine;

public class DamagePassive : PassiveWeaponBase
{
    protected override void Start()
    {
        base.Start();
        AddPassiveStats();
    }

    private void OnDisable()
    {
        RemovePassiveStats();
    }
}
