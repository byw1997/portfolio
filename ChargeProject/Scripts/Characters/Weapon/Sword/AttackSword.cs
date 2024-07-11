using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;
public class AttackSword : AttackWeapon
{
    protected TrailRenderer trail;
    Rigidbody2D rigid;
    List<Collider2D> enemyhit;
    
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        trail = GetComponent<TrailRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        enemyhit = new List<Collider2D>();
        trail.emitting = false;


        weaponid = 2;
        baseknockbackpower = 35f;
        baseknockbacktime = 0.6f;
        basedamage = 20f;
        basedelay = 0.4f;
        initialrotation = 0f;
        lengthdivider = 0f;
        speed = 20f;
        attacktime = 0.15f;
    }
    void OnEnable()
    {
        rigid.velocity = targetdir * speed;
        knockbackpower = baseknockbackpower * knockbackpower_multiplier;
        knockbacktime = baseknockbacktime * knockbacktime_multiplier;
        damage = basedamage * damage_multiplier;
        delay = basedelay * delay_multiplier;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(-targetdir.x, targetdir.y) * 180 / Mathf.PI));
        trail.emitting = true;
    }

    void OnDisable()
    {
        trail.emitting = false;
        enemyhit.Clear();
        rigid.velocity = Vector2.zero;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Enemy") && !enemyhit.Contains(coll))
        {
            IDamageable enemy = coll.GetComponent<IDamageable>();
            if (enemy != null)
            {
                Vector2 dir = (Vector2)coll.transform.position - playerposition;
                enemy.Damage(this.damage, dir.normalized, knockbackpower, knockbacktime);
                enemyhit.Add(coll);
            }
        }
    }

}
