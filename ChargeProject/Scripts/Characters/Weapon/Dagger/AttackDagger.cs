using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDagger : AttackWeapon
{
    protected override void Awake()
    {
        weaponid = 1;
        baseknockbackpower = 0f;
        baseknockbacktime = 0f;
        basedamage = 0f;
        basedelay = 8f;
        initialrotation = 0f;
        lengthdivider = 1f;
        speed = 0f;
        attacktime = 3f;
    }
    void OnEnable()
    {
        this.delay = basedelay * delay_multiplier;
    }
}
