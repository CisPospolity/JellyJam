using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponOption
{
    public WeaponBase weaponPrefab;
    public Sprite icon;
    [TextArea(2, 3)]
    public string description;
}

public class LevelUpManager : MonoBehaviour
{
    [SerializeField] private WeaponPool weaponPool;
    [SerializeField] private GameObject levelUpMenuPrefab;
    [SerializeField] private GameObject activeMenu;

    private WeaponManager weaponManager;
    private bool isChoosingUpgrade;

    private void Awake()
    {
        weaponManager = GetComponent<WeaponManager>();
    }

    public void ShowLevelUpOptions()
    {
        if (isChoosingUpgrade) return;

        Time.timeScale = 0;
        isChoosingUpgrade = true;

        // Instantiate menu if not already present
        if (activeMenu == null)
        {
            activeMenu = Instantiate(levelUpMenuPrefab);
        } else
        {
            activeMenu.SetActive(true);
        }

        var menuUI = activeMenu.GetComponent<LevelUpMenu>();
        menuUI.Initialize(this);

        // Generate 3 random options
        LevelUpOption[] options = GenerateOptions();
        if (options.Length > 0)
        {
            menuUI.ShowOptions(options);
        } else
        {
            Time.timeScale = 1;
            isChoosingUpgrade = false;
            Debug.Log("Nothing to upgrade");
            activeMenu.SetActive(false);
        }
    }

    private LevelUpOption[] GenerateOptions()
    {
        List<LevelUpOption> options = new List<LevelUpOption>();
        List<WeaponBase> currentWeapons = weaponManager.GetCurrentWeapons();

        // Create pool of possible options
        List<LevelUpOption> possibleOptions = new List<LevelUpOption>();

        // Add weapon upgrades
        foreach (var weapon in currentWeapons)
        {
            if (!weapon.IsMaxLevel)
            {
                possibleOptions.Add(new LevelUpOption
                {
                    type = UpgradeType.WeaponUpgrade,
                    weapon = weapon,
                    upgradeLevel = weapon.CurrentLevel + 1,
                    icon = weapon.icon,
                    title = weapon.WeaponName,
                    description = weapon.Upgrades[weapon.CurrentLevel - 1].description
                });
            }
        }

        // Add new weapons
        foreach (var weaponPrefab in weaponPool.AvailableWeapons)
        {
            if (!currentWeapons.Exists(w => w.GetType() == weaponPrefab.GetType()) && weaponManager.CanAddWeapon(weaponPrefab))
            {
                possibleOptions.Add(new LevelUpOption
                {
                    type = UpgradeType.NewWeapon,
                    weaponPrefab = weaponPrefab,
                    icon = weaponPrefab.icon,
                    title = $"New: {weaponPrefab.WeaponName}",
                    description = weaponPrefab.Description
                });
            }
        }
        // Select 3 random options
        while (possibleOptions.Count > 0)
        {
            int index = Random.Range(0, possibleOptions.Count);
            options.Add(possibleOptions[index]);
            possibleOptions.RemoveAt(index);

            // Cap at 3 options maximum
            if (options.Count >= 3) break;
        }

        return options.ToArray();
    }

    public void SelectOption(LevelUpOption option)
    {
        switch (option.type)
        {
            case UpgradeType.WeaponUpgrade:
                option.weapon.LevelUp();
                Debug.Log(string.Format("Upgraded {0} to level {1}", option.weapon.name, option.weapon.CurrentLevel));
                break;

            case UpgradeType.NewWeapon:
                weaponManager.AddWeapon(option.weaponPrefab);
                Debug.Log(string.Format("Gained {0}", option.weaponPrefab.name));
                break;
        }

        Time.timeScale = 1; // Resume the game
        isChoosingUpgrade = false;
        activeMenu.SetActive(false);
    }
}

public enum UpgradeType
{
    WeaponUpgrade,
    NewWeapon
}

public class LevelUpOption
{
    public UpgradeType type;
    public WeaponBase weapon;        // For upgrades
    public WeaponBase weaponPrefab;  // For new weapons
    public int upgradeLevel;
    public Sprite icon;
    public string title;
    public string description;

}
