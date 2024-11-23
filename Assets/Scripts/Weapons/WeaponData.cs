using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public Sprite icon;
    [TextArea(2, 3)]
    public string description;
    public WeaponBase weaponPrefab;
    public WeaponBase.WeaponLevelUpgrade[] levelUpgrades = new WeaponBase.WeaponLevelUpgrade[0];
}
