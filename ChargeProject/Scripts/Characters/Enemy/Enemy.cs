using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Enemy : Character, IDamageable
{
    [SerializeField] protected float speed;
    public float speed_multiplier = 1f;
    protected List<int> SpellOnEffect;
    [SerializeField] protected float basehp;
    public Rigidbody2D target;
    private int stage;
    public float attack = 10f;
    private float hpmultiplier;
    private float attackmultiplier;
    [SerializeField] private bool isknockback;
    private bool canattack = true;
    [SerializeField] private bool knockbackable = true;
    private bool invincible = false;
    
    [SerializeField] protected float knockbackpower = 1f;
    [SerializeField] protected float knockbacktime = 0.5f;

    protected Coroutine knockbackcoroutine = null;
    [SerializeField] protected bool trackable;
    Vector2 targetdir;
    Vector2 targetpos;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        SpellOnEffect = new List<int>();
    }

    void OnEnable()
    {
        SpellOnEffect.Clear();
        stage = GameManager.instance.stage;
        target = GameManager.instance.player.GetRigid();
        this.trackable = GameManager.instance.player.Track();
        if (!this.trackable)
        {
            targetpos = rigid.position;
        }
        else
        {
            targetpos = target.position;
        }
        hpmultiplier = (float)Math.Pow(1.2f, stage-1);
        attackmultiplier = (float)Math.Pow(1.1f, stage - 1);
        maxhp = basehp * hpmultiplier;
        curhp = maxhp;
        isknockback = false;
        knockbackable = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        this.trackable = GameManager.instance.player.Track();
        if (!isknockback)
        {
            if (this.trackable)
            {
                targetpos = target.position;
            }
            targetdir = targetpos - rigid.position;
            targetdir = targetdir.normalized * speed * speed_multiplier * Time.fixedDeltaTime;
            rigid.MovePosition(rigid.position + targetdir);
            rigid.velocity = Vector2.zero;
        }
        
    }
    void LateUpdate()
    {
        sprite.flipX = targetdir.x < 0;
    }

    public IEnumerator KnockBack(Vector2 dir, float knockbackpower, float knockbacktime)
    {
        if (knockbackable)
        {
            isknockback = true;
            canattack = false;
            rigid.AddForce(dir * knockbackpower, ForceMode2D.Impulse);
            yield return new WaitForSeconds(knockbacktime);
            canattack = true;
            isknockback = false;
        }
    }

    public void Damage(float damage, Vector2 dir, float knockbackpower, float knockbacktime)
    {
        if(!invincible)
        {
            curhp = (curhp - damage <= 0f) ? 0f : curhp - damage;
            if (curhp <= 0f)
            {
                Die();
            }
            else
            {
                if (knockbackcoroutine != null)
                {
                    StopCoroutine(knockbackcoroutine);
                    knockbackcoroutine = null;
                }
                StartCoroutine(KnockBack(dir, knockbackpower, knockbacktime));
            }
        }
        
    }
    public void Die()
    {
        gameObject.SetActive(false);
    }

    public float Attack()
    {
        return attack;
    }
    public float KnockbackPower()
    {
        return knockbackpower;
    }
    public float KnockbackTime() 
    {
        return knockbacktime;
    }

    public void Slow(float degree, float time, int spellid)
    {
        if(gameObject.activeSelf && !SpellOnEffect.Contains(spellid))
        {
            StartCoroutine(SlowForSeconds(degree, time, spellid));
        }
    }
    protected IEnumerator SlowForSeconds(float degree, float time, int spellid)
    {
        SpellOnEffect.Add(spellid);
        speed_multiplier *= (1 -degree);
        yield return new WaitForSeconds(time);
        speed_multiplier /= (1 -degree);
        SpellOnEffect.Remove(spellid);
    }
}
