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

    private WeaponManager weaponManager;
    private GameObject activeMenu;
    private bool isChoosingUpgrade;

    private void Awake()
    {
        weaponManager = GetComponent<WeaponManager>();
    }

    public void ShowLevelUpOptions()
    {
        if (isChoosingUpgrade) return;

        Time.timeScale = 0; // Pause the game
       /* isChoosingUpgrade = true;

        // Instantiate menu if not already present
        if (activeMenu == null)
        {
            activeMenu = Instantiate(levelUpMenuPrefab);
            var menuUI = activeMenu.GetComponent<LevelUpMenuUI>();
            menuUI.Initialize(this);
        }*/

        // Generate 3 random options
        LevelUpOption[] options = GenerateOptions();
        if (options.Length > 0)
        {
            SelectOption(options[Random.Range(0, options.Length)]);
        } else
        {
            Time.timeScale = 1;
            Debug.Log("Nothing to upgrade");
        }
        //activeMenu.GetComponent<LevelUpMenuUI>().ShowOptions(options);
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
                    //icon = weapon.GetComponent<WeaponDisplay>()?.Icon,
                    title = weapon.Upgrades[weapon.CurrentLevel - 1].upgradeName,
                    description = weapon.Upgrades[weapon.CurrentLevel - 1].description
                });
            }
        }

        // Add new weapons
        foreach (var weaponPrefab in weaponPool.AvailableWeapons)
        {
            if (!currentWeapons.Exists(w => w.GetType() == weaponPrefab.GetType()))
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
        while (options.Count < 3 && possibleOptions.Count > 0)
        {
            int index = Random.Range(0, possibleOptions.Count);
            options.Add(possibleOptions[index]);
            possibleOptions.RemoveAt(index);
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
        Destroy(activeMenu);
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
