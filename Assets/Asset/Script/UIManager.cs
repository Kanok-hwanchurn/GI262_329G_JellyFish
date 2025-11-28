using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public StatSystem playerStats;

    [Header("UI Elements")]
    public Slider hpBar;
    public Slider expBar;
    public TextMeshProUGUI levelText;

    void Start()
    {
        if (playerStats != null)
        {
            hpBar.maxValue = playerStats.maxHealth;
            expBar.maxValue = playerStats.xpToNextLevel;
        }
    }

    void Update()
    {
        if (playerStats == null) return;

        hpBar.maxValue = playerStats.maxHealth;
        hpBar.value = playerStats.currentHealth;

        expBar.maxValue = playerStats.xpToNextLevel;
        expBar.value = playerStats.currentXP;

        levelText.text = $"Lv. {playerStats.level}";
    }
}
