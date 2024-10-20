using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeSword : ChargeWeapon
{
    void Awake()
    {
        weaponid = 2;
        basechargepower = 45.0f;
        basechargetime = 0.15f;
        basedelay = 0.3f;
        baseknockbackpower = 20f;
        baseknockbacktime = 1.5f;
        basedamage = 60f;
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

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Enemy"))
        {
            IDamageable enemy = coll.GetComponent<IDamageable>();
            if (enemy != null)
            {
                Vector2 dir = coll.transform.position - this.transform.position;
                enemy.Damage(this.damage, dir.normalized, this.knockbackpower, this.knockbacktime);
            }
        }
    }
}
