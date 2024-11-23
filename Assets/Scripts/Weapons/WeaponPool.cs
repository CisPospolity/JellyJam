using UnityEngine;

[CreateAssetMenu(fileName = "WeaponPool", menuName = "Weapons/Weapon Pool")]
public class WeaponPool : ScriptableObject
{
    [SerializeField] private WeaponBase[] availableWeapons;

    public WeaponBase[] AvailableWeapons => availableWeapons;
}
