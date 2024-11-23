using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private WeaponBase startingWeaponPrefab;
    [SerializeField] private int maxAdditionalWeapons = 3;

    [SerializeField] private List<WeaponBase> currentWeapons = new List<WeaponBase>();
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private WeaponBase activeStartingWeapon;

    private void Awake()
    {
        SetupStartingWeapon();
        CollectExistingWeapons();
    }

    private void SetupStartingWeapon()
    {
        if (startingWeaponPrefab == null)
        {
            Debug.LogWarning("No starting weapon prefab assigned!");
            return;
        }

        WeaponBase[] existingWeapons = weaponHolder.GetComponentsInChildren<WeaponBase>(true);
        activeStartingWeapon = existingWeapons.FirstOrDefault(w => w.GetType() == startingWeaponPrefab.GetType());

        if (activeStartingWeapon == null)
        {
            activeStartingWeapon = Instantiate(startingWeaponPrefab, transform);
        }
    }

    private void CollectExistingWeapons()
    {
        currentWeapons.Clear();

        if (activeStartingWeapon != null)
        {
            currentWeapons.Add(activeStartingWeapon);
        }

        WeaponBase[] weapons = weaponHolder.GetComponentsInChildren<WeaponBase>(true);
        foreach (var weapon in weapons)
        {
            if (weapon != activeStartingWeapon && !currentWeapons.Contains(weapon))
            {
                currentWeapons.Add(weapon);
            }
        }
    }

    public bool CanAddWeapon()
    {
        int additionalWeapons = currentWeapons.Count - (activeStartingWeapon != null ? 1 : 0);
        return additionalWeapons < maxAdditionalWeapons;
    }

    public void AddWeapon(WeaponBase weaponPrefab)
    {
        if (!CanAddWeapon())
        {
            Debug.LogWarning("Cannot add weapon: Maximum weapon capacity reached!");
            return;
        }

        if (currentWeapons.Exists(w => w.GetType() == weaponPrefab.GetType()))
        {
            Debug.LogWarning($"Weapon of type {weaponPrefab.GetType()} already exists!");
            return;
        }

        WeaponBase newWeapon = Instantiate(weaponPrefab, weaponHolder.transform);
        newWeapon.name = weaponPrefab.name;
        currentWeapons.Add(newWeapon);
    }

    public List<WeaponBase> GetCurrentWeapons()
    {
        return currentWeapons;
    }

#if UNITY_EDITOR
    public void RefreshWeaponsList()
    {
        if (!Application.isPlaying)
        {
            SetupStartingWeapon();
        }
        CollectExistingWeapons();
    }

    private void OnValidate()
    {
        if (startingWeaponPrefab != null && activeStartingWeapon != null)
        {
            if (activeStartingWeapon.GetType() != startingWeaponPrefab.GetType())
            {
                activeStartingWeapon = null;
            }
        }
    }
#endif
}


#if UNITY_EDITOR

[CustomEditor(typeof(WeaponManager))]
public class WeaponManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WeaponManager manager = (WeaponManager)target;

        EditorGUILayout.Space(10);
        if (GUILayout.Button("Refresh Weapons List"))
        {
            manager.RefreshWeaponsList();
            EditorUtility.SetDirty(manager);
        }

        EditorGUILayout.Space(5);
        EditorGUILayout.HelpBox(
            "Starting weapon will be created automatically if not found in scene.\n" +
            "You can still manually add weapons as children of this object.",
            MessageType.Info);
    }
}
#endif