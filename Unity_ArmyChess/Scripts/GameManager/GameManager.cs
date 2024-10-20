using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Button = UnityEngine.UI.Button;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Board board;
    internal PlayerControlHandler playerControlHandler;
    public Camera mainCamera;
    public CombatManager combatManager;
    internal RewardManager rewardManager;
    [SerializeField] SkillManager skillManager;

    public bool isWhiteCheck;
    public bool isBlackCheck;
    public int chessTurn = -1;//-1: Loading 0: White Turn 1: Black Turn 2: Get White Promotion Input 3: Get Black Promotion Input 4: White Combat 5: Black Combat
    public int playerTeam;

    public float bonusRate;

    public List<PieceUpgrade> pieceUpgrades;
    public List<PieceUpgrade> enemyUpgrades;

    [SerializeField] internal int currentStage;

    List<Reward> selectedRewards;

    internal int rewardChoice = -1;

    [SerializeField] Skill[] allSkills;
    Dictionary<int, Skill> skillDict = new Dictionary<int, Skill>();

    public int skillPoint = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        playerControlHandler = GetComponent<PlayerControlHandler>();
        playerControlHandler.board = board;
        playerControlHandler.combatManager = combatManager;
        rewardManager = GetComponent<RewardManager>();
        
        currentStage = 0;
        selectedRewards = new List<Reward>();
        allSkills = Resources.LoadAll<Skill>("Skill");
        foreach(Skill skill in allSkills)
        {
            if (!skillDict.ContainsKey(skill.id))
            {
                skillDict.Add(skill.id, skill);
            }
            else
            {
                Debug.LogWarning($"Duplicate skill ID found: {skill.id} for skill {skill.skillName}");
            }
        }
        pieceUpgrades = new List<PieceUpgrade>();
        enemyUpgrades = new List<PieceUpgrade>();
        for (int i = 0; i < 6; i++)//pawn - knight - bishop - rook - queen - king
        {
            pieceUpgrades.Add(new PieceUpgrade(skillDict[0]));
        }
        for (int i = 0; i < 6; i++)//pawn - knight - bishop - rook - queen - king
        {
            enemyUpgrades.Add(new PieceUpgrade(skillDict[0]));
        }
    }
    private void Start()
    {
        NextStage();
    }



    private void Update()
    {
        playerControlHandler.GetInput(playerTeam, chessTurn);
    }

    public IEnumerator StartCombat(Square attacker, Square target, System.Action<int> onEnd)
    {
        Debug.Log("StartCombat in GameManager Called");
        int result = 0;
        mainCamera.enabled = false;
        
        Piece white = null;
        Piece black = null;
        Piece attackerPiece = attacker.pieceOnSquare.GetComponent<Piece>();
        Piece targetPiece = target.pieceOnSquare.GetComponent<Piece>();
        int attackerTeam = attackerPiece.team;
        if(attackerTeam == 0)
        {
            white = attackerPiece;
            black = targetPiece;
        }
        else
        {
            white = targetPiece;
            black = attackerPiece;
        }
        Debug.Log("Call Activate in CombatManager");
        yield return StartCoroutine(combatManager.Activate(white, black, attackerTeam, target));
        yield return null;
        Debug.Log("Call EndCombat");
        EndCombat();
        //check which team is terminated. If one of attacker survived and target team terminated, result = 0. Else if target survived, attacker terminated, result = 1. Else if both survived, result = 2. Else if both terminated, result = 3.
        bool attackerAlive = false;
        bool targetAlive = false;
        foreach(Soldier soldier in attackerPiece.soldiers)
        {
            if (soldier.alive)
            {
                attackerAlive = true;
                break;
            }
        }
        foreach(Soldier soldier in targetPiece.soldiers)
        {
            if (soldier.alive)
            {
                targetAlive = true;
                break;
            }
        }
        if (attackerAlive)
        {
            if(targetAlive)
            {
                result = 2;
            }
            else
            {
                result = 0;
            }
        }
        else
        {
            if (targetAlive)
            {
                result = 1;
            }
            else
            {
                result = 3;
            }
        }
        Debug.Log("Call back");
        onEnd(result);
    }

    void EndCombat()
    {
        mainCamera.enabled = true;
    }

    public void StageEnd(int winner)
    {
        if (winner == 0)
        {
            Debug.Log("Checkmate: White win");
            StartCoroutine(GetReward());
        }
        else
        {
            Debug.Log("Checkmate: Black win");
            GameOver();
        }
    }

    IEnumerator GetReward()
    {
        board.Clean();
        SelectedRewardType selected = SelectedRewardType.none;
        //Get Reward
        //Get Random 3 Reward
        while (selectedRewards.Count < 3)
        {
            selectedRewards.Add(rewardManager.GetRandomReward(selected));
        }
        //Show Reward UI
        rewardManager.Activate(selectedRewards);
        //Wait until reward chose
        yield return new WaitUntil(() => (rewardChoice != -1));
        //Deactivate Reward UI
        rewardManager.Deactivate();
        //Add reward for pieceUpgrade
        ApplyRewardToUpgrade(selectedRewards[rewardChoice]);

        //Clear and nextStage
        selectedRewards.Clear();
        rewardChoice = -1;

        //Show Skill Management UI if skill point > 0 and wait for confirm
        if(skillPoint > 0)
        {
            yield return StartCoroutine(skillManager.Activate(pieceUpgrades, skillPoint));
        }

        //Call next stage
        NextStage();
    }

    void ApplyRewardToUpgrade(Reward reward)
    {
        switch (reward.Type)
        {
            case rewardType.skillPoint:
                skillPoint += reward.Skill;
                break;
            case rewardType.newSkill:
                pieceUpgrades[(int)reward.PieceType].AddSkill(skillDict[reward.Skill]);
                break;
            case rewardType.number:
                pieceUpgrades[(int)reward.PieceType].AddSoldier(reward.Number);//Ensure reward for number doesn't appear if that kind of soldier already has max number.
                break;
            default:
                if(reward.PieceType == rewardPieceType.all)
                {
                    foreach(PieceUpgrade upgrade in pieceUpgrades)
                    {
                        upgrade.ModifyStat(reward);
                    }
                }
                else
                {
                    pieceUpgrades[(int)reward.PieceType].ModifyStat(reward);
                }
                break;
        }
    }

    public void NextStage()
    {
        StopAllCoroutines();
        chessTurn = -1;
        playerTeam = 0;
        currentStage++;
        //Add stat for enemyUpgrade
        DebugStringForPieceUpgrade();
        GC.Collect();
        board.CreateBoard(8);
        board.CreatePiece();
        board.SetTeamForPiece();
        board.StartTurn(0);
    }

    public void DebugStringForPieceUpgrade()
    {
        for(int i = 0; i < pieceUpgrades.Count; i++)
        {
            string temp = pieceUpgrades[i].ToString();
            switch (i)
            {
                case 0:
                    temp = "Pawn Upgrades\n" + temp; break;
                case 1:
                    temp = "Knight Upgrades\n" + temp; break;
                case 2:
                    temp = "Bishop Upgrades\n" + temp; break;
                case 3:
                    temp = "Rook Upgrades\n" + temp; break;
                case 4:
                    temp = "Queen Upgrades\n" + temp; break;
                case 5:
                    temp = "King Upgrades\n" + temp; break;
            }
            Debug.Log(temp);
        }
    }

    public void GameOver()
    {
        currentStage = 0;
        foreach(var item in pieceUpgrades)
        {
            item.Clean();
        }
    }

    public void RestartStage()
    {
        Debug.Log("Draw");
        currentStage--;
        NextStage();
    }
}
