using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum SubmarineUpgrade
{
    HULL,
    ENGINE,
    SONAR,
    EXCAVATOR
}


public class UpgradeScreen : MonoBehaviour
{
    public int indexChoice = 0;
    public TextMeshProUGUI[] textChoices;
    public TextMeshProUGUI upgradeDescription;
    public Dictionary<SubmarineUpgrade, int> upgradeCost = new Dictionary<SubmarineUpgrade, int>();
    public Dictionary<SubmarineUpgrade, int> upgradeCount = new Dictionary<SubmarineUpgrade, int>();
    public int baseUpgradeCost = 1000;
    public int costIncreaseEachUpgrade = 1000;
    public int allUpgradeCap = 4;
    private bool init = false;


    void Init()
    {
        upgradeCost.Add(SubmarineUpgrade.HULL, baseUpgradeCost);
        upgradeCost.Add(SubmarineUpgrade.ENGINE, baseUpgradeCost);
        upgradeCost.Add(SubmarineUpgrade.SONAR, baseUpgradeCost);
        upgradeCost.Add(SubmarineUpgrade.EXCAVATOR, baseUpgradeCost);

        upgradeCount.Add(SubmarineUpgrade.HULL, 0);
        upgradeCount.Add(SubmarineUpgrade.ENGINE, 0);
        upgradeCount.Add(SubmarineUpgrade.SONAR, 0);
        upgradeCount.Add(SubmarineUpgrade.EXCAVATOR, 0);

        init = true;
    }

    void OnEnable()
    {
        if (!init)
        {
            Init();
        }
        RefreshAvailability();
        RefreshUpgradeDescription();
    }

    void Update()
    {
        if (Input.GetButtonDown("Vertical"))
        {
            float moveVertical = Input.GetAxis("Vertical");
            if (moveVertical > 0)
            {
                moveVertical = 0;
                indexChoice -= 1;
                if (indexChoice < 0)
                    indexChoice = textChoices.Length - 1;
            }
            else if (moveVertical < 0)
            {
                moveVertical = 0;
                indexChoice += 1;
                if (indexChoice > textChoices.Length - 1)
                    indexChoice = 0;
            }
            RefreshUpgradeChoice();
            RefreshUpgradeDescription();
        }

        if (Input.GetKeyDown(KeyCode.E) ||
            Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.Space))
        {
            ApplyUpgrade();
        }
    }

    public void RefreshUpgradeChoice()
    {
        for (int i = 0; i < textChoices.Length; i++)
        {
            textChoices[i].text = textChoices[i].text.Replace(">", "");
            textChoices[i].text = textChoices[i].text.Replace("<", "");
            textChoices[i].text = textChoices[i].text.Trim();
        }

        string tmp = textChoices[indexChoice].text;
        textChoices[indexChoice].text = $">{tmp}<";
    }

    public void RefreshUpgradeDescription()
    {
        // For some reason, without this, Unity complain that
        // upgradeDescription is unassigned (even though it is).
        if (!upgradeDescription)
        {
            return;
        }
        switch ((SubmarineUpgrade)indexChoice)
        {
            case SubmarineUpgrade.HULL:
                upgradeDescription.text = $"Enhance vessel durability.\n"
                + $"Cost {upgradeCost[SubmarineUpgrade.HULL]} 'score'.\n"
                + $"Upgraded {upgradeCount[SubmarineUpgrade.HULL]}/{allUpgradeCap}.";
                break;
            case SubmarineUpgrade.ENGINE:
                upgradeDescription.text = $"Improve submarine travel speed by 15%.\n"
                + $"Cost {upgradeCost[SubmarineUpgrade.ENGINE]} 'score'.\n"
                + $"Upgraded {upgradeCount[SubmarineUpgrade.ENGINE]}/{allUpgradeCap}.";
                break;
            case SubmarineUpgrade.SONAR:
                upgradeDescription.text = $"Increase sonar rotate speed by 25%.\n"
                + $"Cost {upgradeCost[SubmarineUpgrade.SONAR]} 'score'.\n"
                + $"Upgraded {upgradeCount[SubmarineUpgrade.SONAR]}/{allUpgradeCap}.";
                break;
            case SubmarineUpgrade.EXCAVATOR:
                upgradeDescription.text = $"Boost ore extract efficiency by 25%.\n"
                + $"Cost {upgradeCost[SubmarineUpgrade.EXCAVATOR]} 'score'.\n"
                + $"Upgraded {upgradeCount[SubmarineUpgrade.EXCAVATOR]}/{allUpgradeCap}.";
                break;
            default:
                break;
        }
    }

    public void ApplyUpgrade()
    {
        SubmarineUpgrade choice = (SubmarineUpgrade)indexChoice;
        if (GameManager.instance.score >= upgradeCost[choice] && upgradeCount[choice] < allUpgradeCap)
        {
            GameManager.instance.score -= upgradeCost[choice];
            GameManager.instance.RefreshScoreText();
            upgradeCost[choice] += costIncreaseEachUpgrade;
            upgradeCount[choice] += 1;
        }
        else
        {
            return;
        }

        switch (choice)
        {
            case SubmarineUpgrade.HULL:
                GameManager.instance.currentSubmarineHp += 1;
                GameManager.instance.maxSubmarineHp += 1;
                break;
            case SubmarineUpgrade.ENGINE:
                GameManager.instance.speedLevel += 1;
                GameManager.instance.submarine.GetComponent<SubmarineController>().UpdateSpeedBonus();
                break;
            case SubmarineUpgrade.SONAR:
                GameManager.instance.sonarLevel += 1;
                break;
            case SubmarineUpgrade.EXCAVATOR:
                GameManager.instance.excavatorLevel += 1;
                break;
            default:
                break;
        }

        GameManager.instance.PlayUpgradeSound();
        GameManager.instance.RefreshSideScreen();
        RefreshUpgradeDescription();
        RefreshAvailability();
    }

    public void RefreshAvailability()
    {
        // Change color for text based on whether you can purchase upgrade
        // Color defaultGreenColor = new Color(0.085f, 1, 0, 1);
        for (int i = 0; i < textChoices.Length; i++)
        {
            SubmarineUpgrade choice = (SubmarineUpgrade)i;
            if (GameManager.instance.score >= upgradeCost[choice] && upgradeCount[choice] < allUpgradeCap)
            {
                Color tmp = textChoices[i].color;
                tmp.a = 1;
                textChoices[i].color = tmp;
            }
            else
            {
                Color tmp = textChoices[i].color;
                tmp.a = 0.2f;
                textChoices[i].color = tmp;
            }
        }
    }
}
