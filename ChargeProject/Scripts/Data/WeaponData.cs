using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Object/WeaponData")]
public class WeaponData : ScriptableObject
{
    public enum WeaponType { LongSword, Dagger, Sword, Staff }
    [Header("# Weapon Information")]
    public int weaponid;
    public WeaponType weapontype;
    public string weaponname;
    public string flavortext;
    [Header("# Attack Information")]
    public float attack_baseknockbackpower;
    public float attack_baseknockbacktime;
    public float attack_basedamage;
    public float attack_basedelay;
    public float attack_initialrotation;
    public float attack_lengthdivider;
    public float attack_speed;
    public float attack_attacktime;
    public GameObject attack_prefab;
    [Header("# Charge Information")]
    public float charge_baseknockbackpower;
    public float charge_baseknockbacktime;
    public float charge_basedamage;
    public float charge_basedelay;
    public float charge_chargetime;
    public float charge_chargepower;
    public GameObject charge_prefab;
}
