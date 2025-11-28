using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LevelUpManager : MonoBehaviour
{
    public StatSystem playerStats;
    public GameObject levelUpPanel;

    [System.Serializable]
    public enum StatType
    {
        MaxHealth,
        RegenHP,
        Attack,
        Defense,
        MoveSpeed
    }

    [System.Serializable]
    public class UpgradeDefinition
    {
        public string id;
        public string displayName;
        [TextArea] public string description;
        public StatType statType;
        public float amount;
    }

    [System.Serializable]
    public class UpgradeButtonUI
    {
        public Button button;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI descriptionText;
    }

    [Header("Buttons")]
    public UpgradeButtonUI[] buttons;

    private List<UpgradeDefinition> allUpgrades = new List<UpgradeDefinition>();
    private bool isChoosing = false;

    void Start()
    {
        GenerateDefaultUpgrades();

        if (levelUpPanel != null)
            levelUpPanel.SetActive(false);
    }

    void GenerateDefaultUpgrades()
    {
        allUpgrades.Clear();

        allUpgrades.Add(new UpgradeDefinition {
            id = "hp_10",
            displayName = "Max Health +10",
            description = "Max Health plus 10 ",
            statType = StatType.MaxHealth,
            amount = 10f
        });

        allUpgrades.Add(new UpgradeDefinition {
            id = "regen_0_5",
            displayName = "Regen HP +0.5",
            description = "Regen Faster",
            statType = StatType.RegenHP,
            amount = 0.5f
        });

        allUpgrades.Add(new UpgradeDefinition {
            id = "atk_2",
            displayName = "Attack +2",
            description = "Attack Plus DMG",
            statType = StatType.Attack,
            amount = 2f
        });

        allUpgrades.Add(new UpgradeDefinition {
            id = "def_1",
            displayName = "Defense +1",
            description = "Reduce Damage",
            statType = StatType.Defense,
            amount = 1f
        });

        allUpgrades.Add(new UpgradeDefinition {
            id = "speed_0_5",
            displayName = "Move Speed +0.5",
            description = "Move Faster",
            statType = StatType.MoveSpeed,
            amount = 0.5f
        });
    }

    public void OnPlayerLevelUp(StatSystem stats)
    {
        if (stats != null)
            playerStats = stats;

        ShowChoices();
    }

    void ShowChoices()
    {
        if (levelUpPanel == null || buttons == null || buttons.Length == 0 || allUpgrades.Count == 0)
        {
            Debug.LogWarning("LevelUpManager not configured properly.");
            return;
        }

        levelUpPanel.SetActive(true);
        Time.timeScale = 0f;
        isChoosing = true;

        List<int> available = new List<int>();
        for (int i = 0; i < allUpgrades.Count; i++)
            available.Add(i);

        int optionsToShow = Mathf.Min(buttons.Length, available.Count);

        for (int i = 0; i < buttons.Length; i++)
        {
            if (i < optionsToShow)
            {
                int listIndex = Random.Range(0, available.Count);
                int upgradeIndex = available[listIndex];
                available.RemoveAt(listIndex);

                UpgradeDefinition def = allUpgrades[upgradeIndex];
                var ui = buttons[i];

                if (ui.titleText != null)
                    ui.titleText.text = def.displayName;
                if (ui.descriptionText != null)
                    ui.descriptionText.text = def.description;

                ui.button.gameObject.SetActive(true);
                ui.button.onClick.RemoveAllListeners();
                ui.button.onClick.AddListener(() => OnUpgradeSelected(def));
            }
            else
            {
                buttons[i].button.gameObject.SetActive(false);
            }
        }
    }

    void OnUpgradeSelected(UpgradeDefinition def)
    {
        ApplyUpgrade(def);
        ClosePanel();
    }

    void ApplyUpgrade(UpgradeDefinition def)
    {
        if (playerStats == null) return;

        switch (def.statType)
        {
            case StatType.MaxHealth:
                playerStats.maxHealth += def.amount;
                playerStats.currentHealth += def.amount;
                break;
            case StatType.RegenHP:
                playerStats.regenHP += def.amount;
                break;
            case StatType.Attack:
                playerStats.attack += def.amount;
                break;
            case StatType.Defense:
                playerStats.defense += def.amount;
                break;
            case StatType.MoveSpeed:
                playerStats.moveSpeed += def.amount;
                break;
        }

        Debug.Log($"Applied upgrade: {def.displayName}");
    }

    void ClosePanel()
    {
        if (!isChoosing) return;

        isChoosing = false;
        if (levelUpPanel != null)
            levelUpPanel.SetActive(false);

        Time.timeScale = 1f;
    }
}
