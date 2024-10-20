using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "ScriptableObject/Skill")]
public class Skill : ScriptableObject
{
    public int id;
    public string skillName;
    public int type;
    public bool physical;
    public bool heal;
    public int hitTime;
    public bool randomTarget;
    public bool aoe;
    

    public List<float> baseDamage;
    public List<float> multiplier;
    public List<float> pierce;
    public List<float> pierceRate;

    public bool enemyTarget;
    public bool allyTarget;

    public float CalculateDamage(Soldier attacker, int level, Soldier target)
    {
        float stat;
        if(physical)
        {
            stat = attacker.curAttack;
        }
        else
        {
            stat = attacker.curMagic;
        }
        float damage = baseDamage[level] + stat * multiplier[level];
        if (heal)
        {
            return damage;
        }
        damage *= 1f - target.DefenseRate(physical, pierce[level], pierceRate[level]);

        return damage;
    }

    public override string ToString()
    {
        string ret = string.Empty;
        ret += "Skill ID: " + id + "\nSkill name: " + skillName + "\nisEnemyTarget: " + enemyTarget + "\nisAllyTarget" + allyTarget + "\nisPhysical: " + physical + "\nisHeal: " + heal + "\nhitTime: " + hitTime + "\nisRandom: " + randomTarget + "\nisAOE: " + aoe + "\n";
        return ret;
    }

    public string Desc(int level)
    {
        {
            string info = skillName + " Lv" + (level+1);
            info += heal ? ": Heals " : ": Deals ";
            if (baseDamage[level] > 0) info += baseDamage[level] + " + ";
            info += physical ? "ATK " : "MGC ";
            info += " * " + multiplier[level];
            info += heal ? " health" : " damage";
            if(hitTime > 1)
            {
                info += " " + hitTime + " times";
            }
            info += " to";
            if(enemyTarget && allyTarget)
            {
                info += " selected target";
            }
            else if(enemyTarget)
            {
                info += " enemy target";
            }
            else if(allyTarget)
            {
                info += " ally target";
            }
            return info;
        }
    }
}
