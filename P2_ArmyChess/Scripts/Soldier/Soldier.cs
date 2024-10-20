using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier
{
    public int type;
    internal float maxHp;
    internal float curHp;
    internal float attack;
    internal float curAttack;
    internal float defense;
    internal float curDefense;
    internal float magic;
    internal float curMagic;
    internal float speed;

    public List<Skill> skills;
    public List<int> skillLevel;
    int maxSkill = 4;

    public bool alive;

    public Soldier(int x, float hp, float atk, float def, float mgc, float spd, List<Skill> skills, List<int> skillLevel)
    {
        this.skills = new List<Skill>();
        this.skillLevel = new List<int>();
        alive = true;
        type = x;
        switch (x)
        {
            case 0:
                maxHp = 10 + hp;
                curHp = maxHp;
                attack = 10 + atk;
                curAttack = attack;
                defense = 0 + def;
                curDefense = defense;
                magic = 0 + mgc;
                curMagic = magic;
                speed = 1 + spd;
                this.skills = skills;
                this.skillLevel = skillLevel;
                break;
            case 1:
                maxHp = 20 + hp;
                curHp = maxHp;
                attack = 10 + atk;
                curAttack = attack;
                defense = 1 + def;
                curDefense = defense;
                magic = 0 + mgc;
                curMagic = magic;
                speed = 4 + spd;
                this.skills = skills;
                this.skillLevel = skillLevel;
                break;
            case 2:
                maxHp = 20 + hp;
                curHp = maxHp;
                attack = 10 + atk;
                curAttack = attack;
                defense = 0 + def;
                curDefense = defense;
                magic = 2 + mgc;
                curMagic = magic;
                speed = 2 + spd;
                this.skills = skills;
                this.skillLevel = skillLevel;
                break;
            case 3:
                maxHp = 25 + hp;
                curHp = maxHp;
                attack = 10 + atk;
                curAttack = attack;
                defense = 3 + def;
                curDefense = defense;
                magic = 0 + mgc;
                curMagic = magic;
                speed = 3 + spd;
                this.skills = skills;
                this.skillLevel = skillLevel;
                break;
            case 4:
                maxHp = 30 + hp;
                curHp = maxHp;
                attack = 10 + atk;
                curAttack = attack;
                defense = 2 + def;
                curDefense = defense;
                magic = 0 + mgc;
                curMagic = magic;
                speed = 4 + spd;
                this.skills = skills;
                this.skillLevel = skillLevel;
                break;
            case 5:
                maxHp = 30 + hp;
                curHp = maxHp;
                attack = 10 + atk;
                curAttack = attack;
                defense = 1 + def;
                curDefense = defense;
                magic = 0 + mgc;
                curMagic = magic;
                speed = 2 + spd;
                this.skills = skills;
                this.skillLevel = skillLevel;
                break;
        }
    }

    void AddNewSkill(Skill skill)
    {
        if(skills.Count >= maxSkill){
            RemoveSkill();
        }
        skills.Add(skill);
        skillLevel.Add(0); 
    }

    void RemoveSkill()
    {

    }

    void CopyTo(Soldier target)
    {

    }

    void Clean()
    {
        skills.Clear();
        skillLevel.Clear();
    }


    public void UseSkill(int skillId, Soldier target)
    {
        if (alive)
        {
            Skill skill = skills[skillId];
            int level = skillLevel[skillId];
            float damage = skill.CalculateDamage(this, level, target);
            target.GetDamage(damage);
            Debug.Log(TypeToString() + " used " + skill.skillName + "!" + " Enemy took " + damage + " damage.");
        }
    }

    string TypeToString()
    {
        string temp = "Unknown";
        switch (type)
        {
            case 0:
                temp = "Pawn";
                break;
            case 1:
                temp = "Knight";
                break;
            case 2:
                temp = "Bishop";
                break;
            case 3:
                temp = "Rook";
                break;
            case 4:
                temp = "Queen";
                break;
            case 5:
                temp = "King";
                break;
        }
        return temp;
    }

    public string SoldierInfo()
    {
        string temp = TypeToString();
        temp += "\nmaxHp: " + maxHp + "\ncurHp: " + curHp + "\nattack: " + attack + "\ncurAttack: " + curAttack + "\ndefense: " + defense + "\ncurDefense: " + curDefense + "\nmagic: " + magic + "\ncurMagic: " + curMagic + "\nspeed: " + speed;
        return temp;
    }
    public void GetDamage(float damage)
    {
        curHp -= damage;
        if(curHp <= 0f)
        {
            alive = false;
        }
    }

    public float DefenseRate(bool physical, float pierce, float pierceRate)//return reduced rate. not damage rate.
    {
        float ret;
        float stat;
        //Calculate defense rate. In here, use log_10(10+x) - 1.
        if (physical)
        {
            stat = curDefense;
        }
        else
        {
            stat = curMagic;
        }

        stat = (stat - pierce) * (1 - pierceRate);
        if(stat < 0)
        {
            ret = 0f;
        }
        else
        {
            ret = Mathf.Log10(10f + stat) - 1f;
        }
        
        return ret;
    }
}
