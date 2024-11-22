using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private float speed = 5;
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float health = 100;
    [SerializeField] private int level = 1;
    [SerializeField] private int currentExp = 0;
    [SerializeField] private int expToNextLevel = 5;
    [SerializeField] private float invincibilityCooldown = 0.5f;
    private float nextVulnerableTime = 0f;

    [System.Serializable]
    public class Stats
    {
        [SerializeField] public float damagePercent = 100f;
        [SerializeField] public float areaPercent = 100f;
        [SerializeField] public float cooldownPercent = 100f;
        [SerializeField] public float projectileSpeedPercent = 100f;
        [SerializeField] public int additionalProjectileCount = 0;
        [SerializeField] public float durationPercent = 100f;

        public Stats()
        {

        }

        public Stats(Stats other)
        {
            CopyFrom(other);
        }

        public void CopyFrom(Stats other)
        {
            damagePercent = other.damagePercent;
            areaPercent = other.areaPercent;
            cooldownPercent = other.cooldownPercent;
            projectileSpeedPercent = other.projectileSpeedPercent;
            additionalProjectileCount = other.additionalProjectileCount;
            durationPercent = other.durationPercent;
        }

        public void AddStats(Stats other)
        {
            damagePercent += other.damagePercent;
            areaPercent += other.areaPercent;
            cooldownPercent += other.cooldownPercent;
            projectileSpeedPercent += other.projectileSpeedPercent;
            additionalProjectileCount += other.additionalProjectileCount;
            durationPercent += other.durationPercent;
        }
    }

    [SerializeField] private Stats baseStats = new Stats();
    [SerializeField] private Stats currentStats;

    public float Speed
    {
        get
        {
            return speed;
        }
    }

    public float Health
    {
        get
        {
            return health;
        }
    }

    public delegate void OnStatsChangedDelegate();
    public event OnStatsChangedDelegate OnStatsChanged;

    private void Awake()
    {
        currentStats = new Stats(baseStats);
    }

    public Stats GetCurrentStats()
    {
        return currentStats;
    }

    public void ModifyStats(Stats modifier)
    {
        currentStats.AddStats(modifier);
        OnStatsChanged?.Invoke();
        #if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        #endif
    }

    public void GetHit(int damage)
    {
        if (Time.time < nextVulnerableTime) return;

        health -= damage;
        nextVulnerableTime = Time.time + invincibilityCooldown;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerScript))]
public class PlayerScriptEditor : Editor
{
    PlayerScript.Stats customModifier = new PlayerScript.Stats();
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlayerScript player = (PlayerScript)target;

        EditorGUILayout.Space(5);

        // Custom Stat Modifier Section
        EditorGUI.indentLevel++;

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        customModifier.damagePercent = EditorGUILayout.FloatField("Damage %", 0);
        customModifier.areaPercent = EditorGUILayout.FloatField("Area %", 0);
        customModifier.cooldownPercent = EditorGUILayout.FloatField("Cooldown %", 0);
        customModifier.projectileSpeedPercent = EditorGUILayout.FloatField("Projectile Speed %",0);
        customModifier.additionalProjectileCount = EditorGUILayout.IntField("Additional Projectiles", 0);
        customModifier.durationPercent = EditorGUILayout.FloatField("Duration %", 0);

        EditorGUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Apply Modifications"))
        {
            player.ModifyStats(customModifier);
        }

        if (GUILayout.Button("Reset Modifier"))
        {
            customModifier = new PlayerScript.Stats();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        EditorGUI.indentLevel--;

        EditorGUILayout.Space(5);
    }
}
#endif