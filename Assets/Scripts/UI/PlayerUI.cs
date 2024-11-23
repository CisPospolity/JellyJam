using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private PlayerScript player;
    [SerializeField] private Slider expSlider;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Transform iconHolder;
    [SerializeField] private Image iconPrefab;

    private void Awake()
    {
        if (player == null) return;
        player.OnExpChanged += SetExpSlider;
        player.OnHealthChanged += SetHealthSlider;
        player.GetComponent<WeaponManager>().OnActiveWeaponAdded += AddActiveWeaponIcon;
    }

    public void SetExpSlider(int expValue, int nextLevelValue)
    {
        expSlider.maxValue = nextLevelValue;
        expSlider.value = expValue;
    }
    public void SetHealthSlider(float health, float maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
    }

    public void AddActiveWeaponIcon(WeaponBase weapon)
    {
        Image icon = Instantiate(iconPrefab, iconHolder);
        icon.sprite = weapon.icon;
    }

    private void OnDestroy()
    {
        if (player == null) return;

        player.OnHealthChanged -= SetHealthSlider;
        player.OnExpChanged -= SetExpSlider;
    }
}
