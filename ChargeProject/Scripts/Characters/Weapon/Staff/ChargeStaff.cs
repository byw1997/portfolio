using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeStaff : ChargeWeapon
{
    private Transform parent;
    private float slowdegree;
    private float slowtime;
    private int spellid;
    void Awake()
    {
        weaponid = 3;
        basechargepower = 50f;
        basechargetime = 3f;
        basedelay = 0.5f;
        baseknockbackpower = 0f;
        baseknockbacktime = 0f;
        basedamage = 50f;
        slowdegree = 0.3f;
        slowtime = 0.5f;
        spellid = 0;
    }

    void OnEnable()
    {
        this.parent = transform.parent;
        transform.parent = null;
        this.chargepower = this.basechargepower * this.chargepower_multiplier;
        this.chargetime = this.basechargetime * this.chargetime_multiplier;
        this.delay = this.basedelay * this.delay_multiplier;
        this.knockbackpower = this.baseknockbackpower * this.knockbackpower_multiplier;
        this.knockbacktime = this.baseknockbacktime * this.knockbacktime_multiplier;
        this.damage = this.basedamage * this.damage_multiplier * Time.fixedDeltaTime;
        StartCoroutine(DisableSelf());
    }

    void OnTriggerStay2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Enemy"))
        {
            IDamageable enemy = coll.GetComponent<IDamageable>();
            if (enemy != null)
            {
                Vector2 dir = coll.transform.position - this.transform.position;
                enemy.Damage(this.damage, dir.normalized, this.knockbackpower, this.knockbacktime);
                enemy.Slow(slowdegree, slowtime, spellid);
            }
        }
    }

    public IEnumerator DisableSelf()
    {
        yield return new WaitForSeconds(this.chargetime);
        transform.parent = this.parent;
        transform.localPosition = Vector2.zero;
        gameObject.SetActive(false);
    }
}
