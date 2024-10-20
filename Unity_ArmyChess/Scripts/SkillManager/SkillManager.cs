using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Experimental.GraphView;

public class SkillManager : MonoBehaviour
{
    [SerializeField] GameObject UI;

    [SerializeField] List<Button> pawnButtons;
    [SerializeField] List<Button> knightButtons;
    [SerializeField] List<Button> bishopButtons;
    [SerializeField] List<Button> rookButtons;
    [SerializeField] List<Button> queenButtons;

    [SerializeField] List<TMP_Text> pawnButtonNames;
    [SerializeField] List<TMP_Text> knightButtonNames;
    [SerializeField] List<TMP_Text> bishopButtonNames;
    [SerializeField] List<TMP_Text> rookButtonNames;
    [SerializeField] List<TMP_Text> queenButtonNames;

    [SerializeField] List<Button> pawnIncreaseButtons;
    [SerializeField] List<Button> knightIncreaseButtons;
    [SerializeField] List<Button> bishopIncreaseButtons;
    [SerializeField] List<Button> rookIncreaseButtons;
    [SerializeField] List<Button> queenIncreaseButtons;
    [SerializeField] List<Button> pawnDecreaseButtons;
    [SerializeField] List<Button> knightDecreaseButtons;
    [SerializeField] List<Button> bishopDecreaseButtons;
    [SerializeField] List<Button> rookDecreaseButtons;
    [SerializeField] List<Button> queenDecreaseButtons;

    List<List<Button>> skillSelectButtons;
    List<List<TMP_Text>> skillSelectButtonNames;
    List<List<Button>> increaseButtons;
    List<List<Button>> decreaseButtons;


    [SerializeField] Button confirmButton;

    [SerializeField] TMP_Text skillDesc;
    [SerializeField] TMP_Text leftSkillPoints;

    List<List<int>> currentDecision;

    int skillPoint = 0;

    List<PieceUpgrade> upgrades;

    bool notConfirmed = true;

    private void Awake()
    {
        skillSelectButtons = new List<List<Button>>
        {
            pawnButtons,
            knightButtons,
            bishopButtons,
            rookButtons,
            queenButtons
        };

        skillSelectButtonNames = new List<List<TMP_Text>>
        {
            pawnButtonNames,
            knightButtonNames,
            bishopButtonNames,
            rookButtonNames,
            queenButtonNames
        };

        increaseButtons = new List<List<Button>>
        {
            pawnIncreaseButtons,
            knightIncreaseButtons,
            bishopIncreaseButtons,
            rookIncreaseButtons,
            queenIncreaseButtons
        };

        decreaseButtons = new List<List<Button>>
        {
            pawnDecreaseButtons,
            knightDecreaseButtons,
            bishopDecreaseButtons,
            rookDecreaseButtons,
            queenDecreaseButtons
        };

        currentDecision = new List<List<int>>();
        for(int i = 0; i < 5; i++)
        {
            currentDecision.Add(new List<int> { 0, 0, 0, 0 });
        }
    }
    
    public IEnumerator Activate(List<PieceUpgrade> pieceUpgrades, int sp)
    {
        upgrades = pieceUpgrades;
        skillPoint = sp;
        SetButtons();
        UI.SetActive(true);
        while (notConfirmed)
        {
            yield return null;
        }
        UI.SetActive(false);
        Apply();
        Clean();
    }

    void SetButtons()
    {
        for(int i = 0; i < skillSelectButtons.Count; i++)
        {
            for(int j = 0; j < upgrades[i].Skills.Count; j++)
            {
                SetButtonName(i, j);
                AddSkillButtonListener(i, j);
            }
        }
        confirmButton.onClick.AddListener(() => Confirm());
        leftSkillPoints.text = "SP: " + skillPoint;
    }

    void SetButtonName(int i, int j)
    {
        Skill skill = upgrades[i].Skills[j];
        int level = upgrades[i].SkillLevel[j];
        int tempLevel = currentDecision[i][j];
        skillSelectButtonNames[i][j].text = skill.name + ": " + (level + tempLevel + 1).ToString();
    }

    //i = pieceType, j = skill index
    void Apply()//Apply current decision to upgrade
    {
        for (int i = 0; i < skillSelectButtons.Count; i++)
        {
            for (int j = 0; j < upgrades[i].Skills.Count; j++)
            {
                upgrades[i].SkillLevel[j] += currentDecision[i][j];
            }
        }
        GameManager.instance.skillPoint = skillPoint;
    }

    void AddSkillButtonListener(int i, int j)
    {
        
        skillSelectButtons[i][j].onClick.AddListener(() => ShowSkillDesc(i, j));
        increaseButtons[i][j].onClick.AddListener(() => IncreaseLevel(i, j));
        decreaseButtons[i][j].onClick.AddListener(() => DecreaseLevel(i, j));
    }

    void RemoveSkillButtonListener()
    {
        foreach(var buttons in skillSelectButtons)
        {
            foreach(var button in buttons)
            {
                button.onClick.RemoveAllListeners();
            }
        }
        foreach (var buttons in increaseButtons)
        {
            foreach (var button in buttons)
            {
                button.onClick.RemoveAllListeners();
            }
        }
        foreach (var buttons in decreaseButtons)
        {
            foreach (var button in buttons)
            {
                button.onClick.RemoveAllListeners();
            }
        }
    }

    void IncreaseLevel(int i, int j)//at least 1 skill point left, and can't increase over level 5. (0 - 1 - 2 - 3 - 4)
    {
        if (skillPoint > 0 && upgrades[i].SkillLevel[j] + currentDecision[i][j] < 4)
        {
            skillPoint--;
            currentDecision[i][j]++;
        }
        leftSkillPoints.text = "SP: " + skillPoint;
        ShowSkillDesc(i, j);
        SetButtonName(i, j);
    }

    void DecreaseLevel(int i, int j)//can't decrease under base level or current level
    {
        if (currentDecision[i][j] > 0)
        {
            currentDecision[i][j]--;
            skillPoint++;
        }
        leftSkillPoints.text = "SP: " + skillPoint;
        ShowSkillDesc(i, j);
        SetButtonName(i, j);
    }

    void ShowSkillDesc(int i, int j)//Show description of selected skill under UI
    {
        Skill skill = upgrades[i].Skills[j];
        int level = upgrades[i].SkillLevel[j];
        int tempLevel = currentDecision[i][j];
        skillDesc.text = skill.Desc(level + tempLevel);
        if(tempLevel > 0)
        {
            skillDesc.color = Color.blue;
        }
        else
        {
            skillDesc.color = Color.black;
        }
    }

    void Confirm()//Confirm button
    {
        notConfirmed = false;
    }

    void Clean()
    {
        ClearCurrentDecision();
        notConfirmed = true;
        skillDesc.text = "";
        RemoveSkillButtonListener();
    }

    void ClearCurrentDecision()
    {
        foreach(List<int> list in currentDecision)
        {
            for(int i = 0; i < list.Count; i++)
            {
                list[i] = 0;
            }
        }
    }
}
