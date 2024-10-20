using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

using Button = UnityEngine.UI.Button;
using UnityEngine.UIElements;

[Flags]
internal enum SelectedRewardType
{
    none = 0,
    skillPoint = 1,
    newSkill = 2
}

public class RewardManager : MonoBehaviour
{

    [SerializeField] Reward[] commonRewards;
    [SerializeField] Reward[] rareRewards;
    [SerializeField] Reward[] uniqueRewards;

    [SerializeField] int commonRewardPossibilityBase;
    [SerializeField] int rareRewardPossibilityBase;
    [SerializeField] int uniqueRewardPossibilityBase;

    [SerializeField] GameObject rewardUI;
    [SerializeField] List<Button> rewardButtons;
    [SerializeField] List<TMP_Text> rewardTexts;

    string commonColor = "#A5A5A5";
    string rareColor = "#00A5FF";
    string uniqueColor = "#C200E0";

    private void Awake()
    {
        commonRewards = Resources.LoadAll<Reward>("Reward/Common");
        rareRewards = Resources.LoadAll<Reward>("Reward/Rare");
        uniqueRewards = Resources.LoadAll<Reward>("Reward/Unique");
    }

    internal void Activate(List<Reward> rewards)
    {
        for(int i = 0; i < rewards.Count; i++)
        {
            Reward reward = rewards[i];
            SetButtons(reward, i);
            
        }

        rewardUI.SetActive(true);
    }

    internal void Deactivate()
    {
        RemoveRewardButtonListener();
        rewardUI.SetActive(false);
    }

    void SetButtons(Reward reward, int i)
    {
        string temp = "";
        temp += reward.Rarity.ToString() + "\n\n" + reward.PieceType.ToString() + "\n\n";
        switch (reward.Type)
        {
            case rewardType.skillPoint:
                temp += reward.Skill.ToString() + " " + reward.Type.ToString();
                break;
            case rewardType.newSkill:
                temp += reward.Skill.ToString() + " " + reward.Type.ToString();
                break;
            case rewardType.number:
                temp += reward.Number.ToString() + " " + reward.Type.ToString();
                break;
            default:
                temp += reward.Stat.ToString() + " " + reward.Type.ToString();
                break;
        }
        AddRewardButtonListener(rewardButtons[i], i);
        Color color;
        ColorBlock colorBlock = rewardButtons[i].colors;
        switch (reward.Rarity)
        {
            case rewardRarity.common:
                ColorUtility.TryParseHtmlString(commonColor, out color);
                break;
            case rewardRarity.rare:
                ColorUtility.TryParseHtmlString(rareColor, out color);
                break;
            case rewardRarity.unique:
                ColorUtility.TryParseHtmlString(uniqueColor, out color);
                break;
            default:
                ColorUtility.TryParseHtmlString(commonColor, out color);
                break;
        }
        colorBlock.normalColor = color;
        rewardButtons[i].colors = colorBlock;
        rewardTexts[i].text = temp;
    }

    void AddRewardButtonListener(Button button, int i)
    {
        button.onClick.AddListener(() => SelectReward(i));
    }

    void RemoveRewardButtonListener()
    {
        foreach (Button button in rewardButtons)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    void SelectReward(int i)
    {
        GameManager.instance.rewardChoice = i;
    }

    internal Reward GetRandomReward(SelectedRewardType selected)//return reward, but 3 rewards can have only one newSkill reward and only one skillPoint reward.
    {
        int currentStage = GameManager.instance.currentStage;
        int random = Random.Range(0, 100);
        Reward ret;
        if (random < commonRewardPossibilityBase - 2 * (currentStage - 1))//Possibility for Common rarity reward decreases 2% for each stage
        {
            ret = commonRewards[Random.Range(0, commonRewards.Length)];
            while((selected.HasFlag(SelectedRewardType.skillPoint) && ret.Type == rewardType.skillPoint) || selected.HasFlag(SelectedRewardType.newSkill) && ret.Type == rewardType.newSkill)
            {
                ret = commonRewards[Random.Range(0, commonRewards.Length)];
            }
        }
        else if (random < commonRewardPossibilityBase + rareRewardPossibilityBase - (currentStage - 1))//Possibility for Rare rarity reward increases 1% for each stage
        {
            ret = rareRewards[Random.Range(0, rareRewards.Length)];
            while ((selected.HasFlag(SelectedRewardType.skillPoint) && ret.Type == rewardType.skillPoint) || selected.HasFlag(SelectedRewardType.newSkill) && ret.Type == rewardType.newSkill)
            {
                ret = rareRewards[Random.Range(0, rareRewards.Length)];
            }
        }
        else//Possibility for Unique rarity reward increases 1% for each stage
        {
            ret = uniqueRewards[Random.Range(0, uniqueRewards.Length)];
            while ((selected.HasFlag(SelectedRewardType.skillPoint) && ret.Type == rewardType.skillPoint) || selected.HasFlag(SelectedRewardType.newSkill) && ret.Type == rewardType.newSkill)
            {
                ret = uniqueRewards[Random.Range(0, uniqueRewards.Length)];
            }
        }

        if(ret.Type == rewardType.skillPoint)
        {
            selected |= SelectedRewardType.skillPoint;
        }
        else if(ret.Type == rewardType.newSkill)
        {
            selected |= SelectedRewardType.newSkill;
        }

        return ret;
    }
}
