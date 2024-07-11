using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Weapon : MonoBehaviour
{
    [SerializeField] protected int weaponid;
    [SerializeField] protected float baseknockbackpower;
    [SerializeField] public float knockbackpower_multiplier = 1f;
    [SerializeField] protected float baseknockbacktime;
    [SerializeField] public float knockbacktime_multiplier = 1f;
    [SerializeField] protected float basedamage;
    [SerializeField] public float damage_multiplier = 1f;
    [SerializeField] protected float basedelay;
    [SerializeField] protected float delay_multiplier = 1f;

    protected float knockbackpower;
    protected float knockbacktime;
    protected float damage;
    protected float delay;

    public virtual float Delay()
    {
        return delay;
    }

    public int WeaponID()
    {
        return weaponid;
    }

}
