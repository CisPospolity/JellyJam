using Unity.Collections;
using UnityEditor;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public enum WeaponType
    {
        Active,
        Passive
    }

    [System.Serializable]
    public class WeaponStats
    {
        [SerializeField] public float baseDamage = 1f;
        [SerializeField] public float baseCooldown = 1f;
        [SerializeField] public float baseArea = 1f;
        [SerializeField] public float baseProjectileSpeed = 1f;
        [SerializeField] public int baseProjectileCount = 1;
        [SerializeField] public float baseDuration = 1f;

        public WeaponStats()
        {

        }

        public WeaponStats(bool zeroed)
        {
            if(zeroed)
            {
                baseDamage = 0f;
                baseCooldown = 0f;
                baseArea = 0f;
                baseProjectileSpeed = 0f;
                baseProjectileCount = 0;
                baseDuration = 0f;
            }
        }
    }

    [System.Serializable]
    public class WeaponLevelUpgrade
    {
        [TextArea(2, 3)]
        public string description;
        public WeaponStats statModifiers = new WeaponStats(true);
    }


    [SerializeField] protected WeaponData weaponData;
    [SerializeField] protected WeaponStats baseStats;
    [SerializeField] protected WeaponType weaponType;
    protected WeaponLevelUpgrade[] levelUpgrades;

    protected PlayerScript player;
    protected PlayerScript.Stats cachedStats = new PlayerScript.Stats();
    protected float nextAttackTime;

    [SerializeField, ReadOnly]
    protected int currentLevel = 1;

    public int CurrentLevel => currentLevel;
    public int MaxLevel => levelUpgrades.Length + 1;
    public bool IsMaxLevel => currentLevel >= MaxLevel;
    public WeaponLevelUpgrade[] Upgrades => levelUpgrades;
    public string WeaponName => weaponData.weaponName;
    public Sprite icon => weaponData.icon;
    public string Description => weaponData.description;
    public WeaponType Type => weaponType;

    public virtual void LevelUp()
    {
        if (IsMaxLevel) return;

        WeaponLevelUpgrade upgrade = levelUpgrades[currentLevel -1 ];

        baseStats.baseDamage += upgrade.statModifiers.baseDamage;
        baseStats.baseCooldown += upgrade.statModifiers.baseCooldown;
        baseStats.baseArea += upgrade.statModifiers.baseArea;
        baseStats.baseProjectileSpeed += upgrade.statModifiers.baseProjectileSpeed;
        baseStats.baseProjectileCount += upgrade.statModifiers.baseProjectileCount;
        baseStats.baseDuration += upgrade.statModifiers.baseDuration;

        OnLevelUp(currentLevel + 1);
        currentLevel++;
    }

    protected virtual void OnLevelUp(int newLevel)
    {

    }

    protected virtual void Awake()
    {
        player = GetComponentInParent<PlayerScript>();
        player.OnStatsChanged += UpdateCachedStats;
        UpdateCachedStats();

        levelUpgrades = weaponData.levelUpgrades;
    }

    protected virtual void Start()
    {
        if (currentLevel > 1)
        {
            int levelsToAdd = Mathf.Min(currentLevel, MaxLevel) - currentLevel;
            for (int i = 0; i < levelsToAdd; i++)
            {
                LevelUp();
            }
        }
    }
    private void UpdateCachedStats()
    {
        cachedStats = player.GetCurrentStats();
    }

    protected virtual void Update()
    {
        if (weaponType == WeaponType.Active && Time.time > nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + GetModifiedCooldown();
        }
    }

    protected virtual void Attack()
    {

    }

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

#if UNITY_EDITOR
[CustomEditor(typeof(WeaponBase), true)]
public class WeaponBaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WeaponBase weapon = (WeaponBase)target;

        EditorGUILayout.Space(10);
        if (Application.isPlaying)
        {
            GUI.enabled = !weapon.IsMaxLevel;
            if (GUILayout.Button("Level Up Weapon", GUILayout.Height(30)))
            {
                weapon.LevelUp();
                EditorUtility.SetDirty(weapon);
            }
            GUI.enabled = true;

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField($"Level: {weapon.CurrentLevel}/{weapon.MaxLevel}", EditorStyles.boldLabel);
        }
        else
        {
            EditorGUILayout.HelpBox("Enter Play Mode to level up weapon. Set Current Level above to auto-level when game starts.", MessageType.Info);
        }
    }
}
#endif
