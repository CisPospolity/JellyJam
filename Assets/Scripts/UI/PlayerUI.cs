using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private PlayerScript player;
    [SerializeField] private Slider expSlider;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Transform iconHolder;
    [SerializeField] private Image iconPrefab;
    [SerializeField] private Image ultCooldown;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TMP_Text timerText;

    private void Start()
    {
        if (player == null) return;
        player.OnExpChanged += SetExpSlider;
        player.OnHealthChanged += SetHealthSlider;
        player.OnLevelUp += UpdateCooldown;
        player.GetComponent<PlayerMovement>().OnUlt += UpdateCooldown;
        player.GetComponent<WeaponManager>().OnActiveWeaponAdded += AddActiveWeaponIcon;
        gameManager.OnTimerUpdate += UpdateText;
    }

    private void UpdateText(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        var text = string.Format("{0:00}:{1:00}", minutes, seconds);
        timerText.text = text;
    }

    public void UpdateCooldown()
    {
        ultCooldown.fillAmount = player.GetComponent<PlayerMovement>().LeftToUltRatio;
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
        player.OnLevelUp -= UpdateCooldown;
        player.GetComponent<PlayerMovement>().OnUlt -= UpdateCooldown;
        gameManager.OnTimerUpdate -= UpdateText;

    }
}
