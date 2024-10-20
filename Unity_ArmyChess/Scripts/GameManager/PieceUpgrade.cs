using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceUpgrade
{
    int number;
    int maxSoldier = 3;
    public int Number
    {
        get { return number; }
        private set { number = value; }
    }
    float hp;
    public float Hp
    {
        get { return hp;}
        private set { hp = value; }
    }
    float attack;
    public float Attack
    {
        get { return attack; }
        private set { attack = value; }
    }
    float defense;
    public float Defense
    {
        get { return defense; }
        private set { defense = value; }
    }
    float magic;
    public float Magic
    {
        get { return magic; }
        private set { magic = value; }
    }
    float speed;
    public float Speed
    {
        get { return speed; }
        private set { speed = value; }
    }

    List<Skill> skills;
    List<int> skillLevel;
    int maxSkill = 4;
    public List<Skill> Skills
    {
        get { return skills; }
        private set { skills = value; }
    }
    public List<int> SkillLevel
    {
        get { return skillLevel; }
        private set { skillLevel = value; }
    }

    internal PieceUpgrade(Skill baseSkill)
    {
        number = 1;
        hp = 0;
        attack = 0;
        defense = 0;
        magic = 0;
        speed = 0;
        skills = new List<Skill>();
        skillLevel = new List<int>();
        skills.Add(baseSkill);
        skillLevel.Add(0);
    }

    internal void Clean()
    {
        number = 1;
        hp = 0;
        attack = 0;
        defense = 0;
        magic = 0;
        speed = 0;
        skills.Clear();
        skillLevel.Clear();
    }

    public void AddSoldier(int amount)
    {
        number = (number + amount > maxSoldier) ? maxSoldier : number + amount;
    }

    public void DecreaseSoldier()
    {
        if(number > 1)
        {
            number--;
        }
    }

    public void ModifyStat(Reward reward)
    {
        switch (reward.Type)
        {
            case rewardType.hp:
                ModifyHp(reward.Stat);
                break;
            case rewardType.atk:
                ModifyAttack(reward.Stat);
                break;
            case rewardType.def:
                ModifyDefense(reward.Stat);
                break;
            case rewardType.mgc:
                ModifyMagic(reward.Stat);
                break;
            case rewardType.spd:
                ModifySpeed(reward.Stat);
                break;
        }
    }

    void ModifyHp(float amount)
    {
        hp += amount;
    }

    void ModifyAttack(float amount)
    {
        attack += amount;
    }

    void ModifyDefense(float amount)
    {
        defense += amount;
    }

    void ModifyMagic(float amount)
    {
        magic += amount;
    }

    void ModifySpeed(float amount)
    {
        speed   += amount;
    }

    public void AddSkill(Skill skill)
    {
        if (skills.Count < maxSkill)
        {
            skills.Add(skill);
            skillLevel.Add(0);
        }
    }

    public void RemoveSkill(int id)
    {
        skills.RemoveAt(id);
        skillLevel.RemoveAt(id);
    }


    public override string ToString()
    {
        string ret = string.Empty;
        ret += "\nNumber of soldiers: " + number + "\nHp bonus: " + hp + "\nAttack bonus: " + attack + "\nDefense bonus: " + defense + "\nMagic bonus: " + magic + "\nSpeed bonus: " + speed + "\n";
        for(int i = 0; i < skills.Count; i++)
        {
            ret += "Skill " + i + "\nLevel: " + skillLevel[i] + "\n";
            ret += skills[i].ToString();
        }

        return ret;
    }
}
