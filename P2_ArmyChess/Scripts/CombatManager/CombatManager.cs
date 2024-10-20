using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static CombatManager;

public class CombatManager : MonoBehaviour
{
    /*public Transform whiteLoc1;
    public Transform whiteLoc2;
    public Transform whiteLoc3;
    public Transform blackLoc1;
    public Transform blackLoc2;
    public Transform blackLoc3;*/

    public List<SoldierLocation> locations;
    public List<Slider> hpBars;
    public List<TMP_Text> commandReady;

    public Camera combatCamera;
    [SerializeField] GameObject skillUI;
    [SerializeField] List<Button> skillButtons;
    [SerializeField] Button cancelButton;
    [SerializeField] List<TMP_Text> skillNames;

    public List<Soldier> whiteSoldier;
    List<Command> whiteCommands;
    public List<Soldier> blackSoldier;
    List<Command> blackCommands;
    List<(Soldier soldier, Command command)> entireCommand = new List<(Soldier, Command)>();

    public int soldierSelected = -1;
    public int skillSelected = -1;

    internal class Command
    {
        internal int attackerTeam;
        internal int attacker;
        internal int targetTeam;
        internal int target;
        internal int skill;

        internal Command()
        {
            attackerTeam = -1;
            attacker = -1;
            targetTeam = -1;
            target = -1;
            skill = -1;
        }

        internal void SetCommand(int attackerTeam, int attackerId, int targetTeam, int targetId, int skill)
        {
            this.attackerTeam = attackerTeam;
            this.attacker = attackerId;
            this.targetTeam = targetTeam;
            this.target = targetId;
            this.skill = skill;
        }

        internal void Clean()
        {
            attackerTeam = -1;
            attacker = -1;
            targetTeam = -1;
            target = -1;
            skill = -1;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        combatCamera.enabled = false;
        whiteCommands = new List<Command>();
        blackCommands = new List<Command>();
        for(int i = 0; i < 3; i++)
        {
            whiteCommands.Add(new Command());
            blackCommands.Add(new Command());
        }
        for(int i = 0; i < 6; i++)
        {
            Vector3 pos = combatCamera.WorldToScreenPoint(locations[i].transform.position - new Vector3(0,0.2f,0));
            hpBars[i].GetComponent<RectTransform>().position = pos;
            pos = combatCamera.WorldToScreenPoint(locations[i].transform.position + new Vector3(0, 1f, 0));
            commandReady[i].GetComponent<RectTransform>().position = pos;
        }
    }

    public IEnumerator Activate(Piece white, Piece black, int attackerTeam, Square square)
    {
        combatCamera.enabled = true;
        whiteSoldier = white.soldiers;
        blackSoldier = black.soldiers;
        for (int i = 0; i < whiteSoldier.Count; i++)
        {
            whiteCommands.Add(new Command());
        }
        for (int i = 0; i < blackSoldier.Count; i++)
        {
            blackCommands.Add(new Command());
        }
        //Place Soldiers on stage
        PlaceSoldier(square, attackerTeam);
        //Show Skill UI
        skillUI.SetActive(true);
        //Get Command
        GameManager.instance.chessTurn += 4;
        bool loop = true;
        while (loop)
        {
            Debug.Log("Command not over");
            yield return null;
            loop = false;
            for (int i = 0; i < whiteSoldier.Count; i++)
            {
                if (whiteCommands[i].attacker == -1)
                {
                    loop = true;
                    break;
                }
            }
            if (loop) continue;
            for (int i = 0; i < blackSoldier.Count; i++)
            {
                if (blackCommands[i].attacker == -1)
                {
                    loop = true;
                    break;
                }
            }
        }
        GameManager.instance.chessTurn -= 4;
        //Sort Actions
        for (int i = 0; i < whiteSoldier.Count; i++)
        {
            entireCommand.Add((whiteSoldier[i], whiteCommands[i]));
        }
        for (int i = 0; i < blackSoldier.Count; i++)
        {
            entireCommand.Add((blackSoldier[i], blackCommands[i]));
        }
        Shuffle(entireCommand);
        entireCommand.Sort((s1, s2) =>
        {
            if (s1.soldier.speed != s2.soldier.speed)
            {
                return s2.soldier.speed.CompareTo(s1.soldier.speed);
            }
            else if (whiteSoldier.Contains(s1.soldier) && blackSoldier.Contains(s2.soldier))
            {
                return 2 * attackerTeam - 1; // -1 if attacker ==  0, 1 if attacker == 1
            }
            else if (blackSoldier.Contains(s1.soldier) && whiteSoldier.Contains(s2.soldier))
            {
                return 1 - 2 * attackerTeam;
            }
            return 0;
        });
        //Use Skills
        for(int i = 0; i < entireCommand.Count; i++)
        {
            yield return StartCoroutine(UseSkill(entireCommand[i].soldier, entireCommand[i].command));
            yield return new WaitForSeconds(0.5f);
            //If one team has terminated, end skill phase.
            bool battleOver = true;
            List<Soldier> targetSoldier;
            if (entireCommand[i].command.targetTeam == 0)
            {
                targetSoldier = whiteSoldier;
            }
            else
            {
                targetSoldier= blackSoldier;
            }
            foreach(Soldier soldier in targetSoldier)
            {
                if (soldier.alive)
                {
                    battleOver = false;
                }
            }
            if(battleOver)
            {
                break;
            }
        }
        yield return new WaitForSeconds(0.5f);
        //Deactivate
        Deactivate();
        yield return null;
    }
    void PlaceSoldier(Square square, int attackerTeam)
    {
        Debug.Log("Battle on " + square.SquareInfo());
        for(int i = 0; i < whiteSoldier.Count; i++)
        {
            if (whiteSoldier[i].alive)
            {
                whiteSoldier[i].curAttack = whiteSoldier[i].attack + square.whiteAtkBonus;
                whiteSoldier[i].curDefense = whiteSoldier[i].defense + square.whiteDefBonus;
                if(attackerTeam == 0)//Get rid of bonus by self
                {
                    whiteSoldier[i].curAttack -= GameManager.instance.bonusRate * whiteSoldier[i].attack;
                    whiteSoldier[i].curDefense -= GameManager.instance.bonusRate * whiteSoldier[i].defense;
                }
                locations[i].ActivateSoldier(whiteSoldier[i].type);
                hpBars[i].gameObject.SetActive(true);
                hpBars[i].value = whiteSoldier[i].curHp / whiteSoldier[i].maxHp;
                Debug.Log(i + "th white soldier Info:\n" + whiteSoldier[i].SoldierInfo());
            }
        }
        for(int i = 0;i < blackSoldier.Count; i++)
        {
            if (blackSoldier[i].alive)
            {
                blackSoldier[i].curAttack = blackSoldier[i].attack + square.blackAtkBonus;
                blackSoldier[i].curDefense = blackSoldier[i].defense + square.blackDefBonus;
                if (attackerTeam == 1)
                {
                    blackSoldier[i].curAttack -= GameManager.instance.bonusRate * blackSoldier[i].attack;
                    blackSoldier[i].curDefense -= GameManager.instance.bonusRate * blackSoldier[i].defense;
                }
                locations[i + 3].ActivateSoldier(blackSoldier[i].type);
                hpBars[i + 3].gameObject.SetActive(true);
                hpBars[i + 3].value = blackSoldier[i].curHp / blackSoldier[i].maxHp;
                Debug.Log(i + "th black soldier Info:\n" + blackSoldier[i].SoldierInfo());
            }
        }
    }

    IEnumerator UseSkill(Soldier soldier, Command command)
    {
        GameObject soldierObject = locations[command.attackerTeam * 3 + command.attacker].ActiveObject;
        Vector3 start = soldierObject.transform.position;
        Vector3 end = (command.attackerTeam == 0) ? soldierObject.transform.position + new Vector3(0.2f, 0, 0) : soldierObject.transform.position - new Vector3(0.2f, 0, 0);
        float duration = 0.2f;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            soldierObject.transform.position = Vector3.Lerp(start, end, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return null;
        Soldier target;
        if(command.targetTeam == 0)
        {
            target = whiteSoldier[command.target];
        }
        else
        {
            target = blackSoldier[command.target];
        }
        soldier.UseSkill(command.skill, target);
        hpBars[command.attackerTeam * 3 + command.attacker].value = soldier.curHp / soldier.maxHp;
        hpBars[command.targetTeam * 3 + command.target].value = target.curHp / target.maxHp;
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            soldierObject.transform.position = Vector3.Lerp(end, start, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    void Deactivate()
    {
        combatCamera.enabled = false;
        RemoveSoldier();
        entireCommand.Clear();
        for(int i = 0; i < 3; i++)
        {
            whiteCommands[i].Clean();
            blackCommands[i].Clean();
        }
        skillUI.SetActive(false);
        StopAllCoroutines();

    }
    public void RemoveSoldier()
    {
        for(int i = 0; i < 6; i++)
        {
            locations[i].DeactivateSoldier();
            hpBars[i].gameObject.SetActive(false);
            commandReady[i].gameObject.SetActive(false);
        }
    }

    void Shuffle<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public void SelectSoldier(int soldierId)
    {
        soldierSelected = soldierId;

        //Show skill on UI
        ShowSkill();
    }

    void ShowSkill()
    {
        int team = soldierSelected / 3;
        int id = soldierSelected % 3;
        List<Skill> skills;
        if (team == 0)
        {
            skills = whiteSoldier[id].skills;
        }
        else
        {
            skills = blackSoldier[id].skills;
        }
        //Show skills on UI
        for(int i = 0; i < skills.Count; i++)
        {
            AddSkillButtonListener(skillButtons[i], i);
            skillNames[i].text = skills[i].name;
        }
        cancelButton.onClick.AddListener(() => CancelCommand(soldierSelected));
    }

    void UnshowSkill()
    {
        RemoveSkillButtonListener();
        foreach (TMP_Text skillName in skillNames)
        {
            skillName.text = "Empty";
        }
    }

    void AddSkillButtonListener(Button button, int skillId)
    {
        button.onClick.AddListener(() => SelectSkill(skillId));
    }

    void RemoveSkillButtonListener()
    {
        cancelButton.onClick.RemoveAllListeners();
        foreach(Button button in skillButtons)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    public void UnselectSoldier()
    {
        soldierSelected = -1;
        skillSelected = -1;
        UnshowSkill();
    }

    void SelectSkill(int skillId)
    {
        skillSelected = skillId;
    }

    void UnselectSkill()
    {
        skillSelected = -1;
    }

    public void ConfirmCommand(int attackerTeam, int attackerId, int targetTeam, int targetId, int skillId)
    {
        List<Command> commands;
        if(attackerTeam == 0)
        {
            commands = whiteCommands;
        }
        else
        {
            commands = blackCommands;
        }
        commands[attackerId].SetCommand(attackerTeam, attackerId, targetTeam, targetId, skillId);
        commandReady[attackerTeam * 3 + attackerId].gameObject.SetActive(true);
        UnselectSoldier();
    }

    void CancelCommand(int soldierId)
    {
        int targetTeam = soldierId / 3;
        int targetId = soldierId % 3;
        List<Command> commands;
        if (targetTeam == 0)
        {
            commands = whiteCommands;
        }
        else
        {
            commands = blackCommands;
        }
        commandReady[soldierId].gameObject.SetActive(false);
        commands[targetId].Clean();
    }
}
