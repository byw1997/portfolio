using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ChargeDagger : ChargeWeapon
{
    void Awake()
    {
        weaponid = 1;
        basechargepower = 120.0f;
        basechargetime = 0.05f;
        basedelay = 0.25f;
        baseknockbackpower = 0.2f;
        baseknockbacktime = 0.75f;
        basedamage = 125f;
    }

    void OnEnable()
    {
        this.chargepower = this.basechargepower * this.chargepower_multiplier;
        this.chargetime = this.basechargetime * this.chargetime_multiplier;
        this.delay = this.basedelay * this.delay_multiplier;
        this.knockbackpower = this.baseknockbackpower * this.knockbackpower_multiplier;
        this.knockbacktime = this.baseknockbacktime * this.knockbacktime_multiplier;
        this.damage = this.basedamage * this.damage_multiplier;
    }
    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Enemy"))
        {
            IDamageable enemy = coll.GetComponent<IDamageable>();
            if (enemy != null)
            {
                Vector2 dir = coll.transform.position - this.transform.position;
                dir = dir.normalized;
                enemy.Damage(this.damage, dir, this.knockbackpower, this.knockbacktime);
                gameObject.SetActive(false);
            }
        }
    }
}
