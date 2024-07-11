using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeWeapon : Weapon
{

    [SerializeField] protected float basechargepower;
    [SerializeField] public float chargepower_multiplier = 1f;
    [SerializeField] protected float basechargetime;
    [SerializeField] public float chargetime_multiplier = 1f;

    public Vector2 chargedir;
    protected Vector2 knockbackdir;
    protected float chargepower;
    protected float chargetime;

    public virtual float ChargePower()
    {
        return this.chargepower;
    }
    public virtual float ChargeTime()
    {
        return this.chargetime;
    }
    public void Modify_ChargePower(float m)
    {
        this.chargepower_multiplier *= m;
    }
    public void Modify_ChargeTime(float m)
    {
        this.chargetime_multiplier *= m;
    }
}
