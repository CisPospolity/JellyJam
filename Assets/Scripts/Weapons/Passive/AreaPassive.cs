using UnityEngine;

public class AreaPassive : PassiveWeaponBase
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
