using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class AttackLongsword : AttackWeapon
{
    private float attackangle = 180f;
    protected TrailRenderer trail;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        trail = GetComponent<TrailRenderer>();
        trail.emitting = false;
        

        weaponid = 0;
        baseknockbackpower = 25f;
        baseknockbacktime = 0.4f;
        basedamage = 30f;
        basedelay = 0.4f;
        initialrotation = 120f;
        lengthdivider = 0.75f;
        speed = -1440f;
        attacktime = Mathf.Abs(attackangle / speed);
    }
    void OnEnable()
    {
        Debug.Log(this.name);
        trail.emitting = true;
        knockbackpower = baseknockbackpower * knockbackpower_multiplier;
        knockbacktime = baseknockbacktime * knockbacktime_multiplier;
        damage = basedamage * damage_multiplier;
        delay = basedelay * delay_multiplier;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }

    void OnDisable()
    {
        trail.emitting = false;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        transform.RotateAround(playerposition, Vector3.forward, speed * Time.fixedDeltaTime);
        Vector2 rotatedir = playerposition - (Vector2)this.transform.position;
        this.transform.rotation = Quaternion.Euler(new Vector3(0,0,Mathf.Atan2(rotatedir.y, rotatedir.x) * 180 / Mathf.PI));
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Enemy"))
        {
            IDamageable enemy = coll.GetComponent<IDamageable>();
            if (enemy != null)
            {
                Vector2 dir = (Vector2)coll.transform.position - playerposition;
                enemy.Damage(this.damage, dir.normalized, knockbackpower,knockbacktime);
                
            }
        }
    }
}
