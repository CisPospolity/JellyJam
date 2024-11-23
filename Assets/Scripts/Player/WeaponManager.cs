using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private WeaponBase startingWeaponPrefab;
    [SerializeField] private int maxAdditionalWeapons = 3;
    [SerializeField] private int maxPassiveWeapons = 3;

    [SerializeField] private List<WeaponBase> currentWeapons = new List<WeaponBase>();
    [SerializeField] private List<WeaponBase> currentActiveWeapons = new List<WeaponBase>();
    [SerializeField] private List<PassiveWeaponBase> currentPassiveWeapons = new List<PassiveWeaponBase>();
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private WeaponBase activeStartingWeapon;

    public event Action<WeaponBase> OnActiveWeaponAdded;

    private void Start()
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
        OnActiveWeaponAdded?.Invoke(activeStartingWeapon);
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
                if(weapon.Type == WeaponBase.WeaponType.Active)
                {
                    currentActiveWeapons.Add(weapon);
                } else
                {
                    currentPassiveWeapons.Add((PassiveWeaponBase)weapon);
                }
            }
        }
    }

    public bool CanAddWeapon(WeaponBase weaponPrefab)
    {
        if (weaponPrefab.Type == WeaponBase.WeaponType.Passive)
        {
            int currentPassiveCount = currentPassiveWeapons.Count;
            return currentPassiveCount < maxPassiveWeapons;
        }
        else
        {
            int additionalWeapons = currentActiveWeapons.Count - (activeStartingWeapon != null ? 1 : 0);
            return additionalWeapons < maxAdditionalWeapons;
        }
    }

    public void AddWeapon(WeaponBase weaponPrefab)
    {
        if (!CanAddWeapon(weaponPrefab))
        {
            Debug.LogWarning("Cannot add weapon: Maximum weapon capacity reached!");
            return;
        }

        if (weaponPrefab.Type == WeaponBase.WeaponType.Passive)
        {
            if (currentPassiveWeapons.Exists(w => w.GetType() == weaponPrefab.GetType()))
            {
                Debug.LogWarning($"Passive weapon of type {weaponPrefab.GetType()} already exists!");
                return;
            }

            PassiveWeaponBase newPassive = Instantiate(weaponPrefab, weaponHolder.transform) as PassiveWeaponBase;
            newPassive.name = weaponPrefab.name;
            currentPassiveWeapons.Add(newPassive);
        }
        else
        {
            if (currentWeapons.Exists(w => w.GetType() == weaponPrefab.GetType()))
            {
                Debug.LogWarning($"Weapon of type {weaponPrefab.GetType()} already exists!");
                return;
            }

            WeaponBase newWeapon = Instantiate(weaponPrefab, weaponHolder.transform);
            newWeapon.name = weaponPrefab.name;
            currentWeapons.Add(newWeapon);
            OnActiveWeaponAdded?.Invoke(newWeapon);
        }
    }

    public List<WeaponBase> GetCurrentWeapons()
    {
        return currentWeapons;
    }
    public List<PassiveWeaponBase> GetCurrentPassiveWeapons()
    {
        return currentPassiveWeapons;
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