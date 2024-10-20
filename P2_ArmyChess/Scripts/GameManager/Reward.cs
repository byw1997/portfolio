using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum rewardRarity
{
    common,
    rare,
    unique
}

public enum rewardPieceType
{
    pawn,
    knight,
    bishop,
    rook,
    queen,
    king,
    all
}

public enum rewardType
{
    hp,
    atk,
    def,
    mgc,
    spd,
    skillPoint,
    newSkill,
    number
}

[CreateAssetMenu(fileName = "NewReward", menuName = "ScriptableObject/Reward")]



public class Reward : ScriptableObject
{
    
    [SerializeField] rewardRarity rarity;
    public rewardRarity Rarity
    {
        get { return rarity; }
        private set { rarity = value; }
    }
    
    [SerializeField] rewardPieceType pieceType;//0: Pawn 1: Knight 2: Bishop 3: Rook 4: Queen 5: King 6: All Piece
    public rewardPieceType PieceType
    {
        get { return pieceType; }
        private set { pieceType = value; }
    }

    
    [SerializeField] rewardType type;//0: Hp 1: Atk 2: Def 3: Mgc 4: Spd 5: Skill Point 6: New Skill
    public rewardType Type
    {
        get { return type; }
        private set { type = value; }
    }

    [SerializeField] float stat;//-1 if new skill or skill point.
    public float Stat
    {
        get { return stat; }
        private set { stat = value; }
    }

    [SerializeField] int skill;//-1 if stat bonus. number of skill point if skill point, skill id if new skill.
    public int Skill
    {
        get { return skill; }
        private set { skill = value; }
    }

    [SerializeField] int number;//-1 if not number.
    public int Number
    {
        get { return number; }
        private set { number = value; }
    }
}
