using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Object/EnemyData")]
public class EnemyData : ScriptableObject
{
    public enum EnemyType { Melee, Range, Turret, Boss }
    [Header("# Information")]
    public EnemyType enemytype;
    public int enemyid;
    public string enemyname;
    public string flavortext;

    [Header("# Spec")]
    public float basedamage;
    public float basehealth;
    public float basespeed;
}
