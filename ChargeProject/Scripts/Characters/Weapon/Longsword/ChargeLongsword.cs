using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
public class ChargeLongsword : ChargeWeapon
{
    void Awake()
    {
        weaponid = 0;
        basechargepower = 75.0f;
        basechargetime = 0.1f;
        basedelay = 0.6f;
        baseknockbackpower = 30f;
        baseknockbacktime = 1f;
        basedamage = 150f;
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
                if(chargedir.x * dir.y - chargedir.y * dir.x > 0)//¿ÞÂÊ
                {
                    knockbackdir.x = -chargedir.y;
                    knockbackdir.y = chargedir.x;
                }
                else
                {
                    knockbackdir.x = chargedir.y;
                    knockbackdir.y = -chargedir.x;
                }
                enemy.Damage(this.damage, this.knockbackdir, this.knockbackpower, this.knockbacktime);
            }
        }
    }
}
