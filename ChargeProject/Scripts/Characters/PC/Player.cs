using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class Player : Character, IDamageable
{

    private static Player instance;
    internal PlayerCharge playercharge;
    internal PlayerAttack playerattack;
    internal PlayerInput playerinput;
    internal PlayerMovement playermovement;
    [SerializeField] internal WeaponManager weaponmanager;
    internal CapsuleCollider2D collider;
    public List<int> SpellOnEffect;
    public static Player Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Player>();
            }
            return instance;
        }
    }
    public bool trackable;
    public Vector2 input_vector;
    [SerializeField] private Slider hpbar;
    [SerializeField] private Slider stbar;

    [SerializeField] internal float basespeed;
    public float speed_multiplier;
    internal float speed;

    public float maxst;
    public float curst;
    private float stgen = 0.5f;
    private float stgeninterval = 0.05f;
    public float dodgest = 15f;
    public float chargest = 100f;

    internal TrailRenderer trail;
    
    public float chargespeed;
    public Vector2 mouseDir;

    internal bool canmove = true;

    public float dodgepower;
    public float dodgetime;
    public float dodgecooltime;
    internal bool candodge = false;
    internal bool isdodge;

    internal bool knockbackable = true;
    internal bool invincible = false;
    internal float invincibletime = 0f;
    internal float damagedelay = 1.25f;

    public GameObject[] chargeprefabs;
    internal GameObject mainchargeobject = null;
    internal ChargeWeapon maincharge = null;
    internal GameObject subchargeobject = null;
    internal ChargeWeapon subcharge = null;

    public GameObject[] attackprefabs;
    internal GameObject mainattackobject = null;
    internal AttackWeapon mainattack = null;
    internal GameObject subattackobject = null;
    internal AttackWeapon subattack = null;

    internal bool cancharge = false;
    internal bool ischarge;


    internal bool canattack = true;
    internal bool isattack;

    internal bool isknockback;

    public float damage_multiplier = 1f;
    internal bool swordpowercharging = false;
    internal float swordpowerchargetime = 0f;
    internal float maxswordpowerchargetime = 3f;

    private Coroutine chargecoroutine;
    private Coroutine attackcoroutine;
    // Start is called before the first frame update
    protected override void Awake()
    {
        
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            base.Awake();
            trail = GetComponent<TrailRenderer>();
            playercharge = GetComponent<PlayerCharge>();
            playerattack = GetComponent<PlayerAttack>();
            playerinput = GetComponent<PlayerInput>();
            playermovement = GetComponent<PlayerMovement>();
            collider = GetComponent<CapsuleCollider2D>();
            hpbar.value = 1f;
            maxhp = 100f;
            curhp = 100f;
            maxst = 100f;
            curst = 0f;
            basespeed = 4.5f;
            speed_multiplier = 1f;
            speed = basespeed * speed_multiplier;
            stbar.value = 0f;
            isknockback = false;
            instance = this;
            trackable = true;
            SpellOnEffect = new List<int>();
            DontDestroyOnLoad(gameObject);
        }
    }
    void Start()
    {
        StartCoroutine(StaminaRegen());
    }
    // Update is called once per frame
    void Update()
    {
        playerinput.GetInput();
    }

    void FixedUpdate()
    {
        playermovement.Move();
        
    }

    void LateUpdate() 
    {
        anim.SetFloat("Speed", input_vector.magnitude);
        if (canmove)
        {
            if (curst >= chargest)
            {
                cancharge = true;
            }
            else
            {
                cancharge = false;
            }
            if (curst >= dodgest)
            {
                candodge = true;
            }
            else
            {
                candodge = false;
            }
        }
        sprite.flipX = mouseDir.x < 0;
    }
    public void TraceCursor()
    {
        Vector3 mousepos = Input.mousePosition;
        mousepos = Camera.main.ScreenToWorldPoint(mousepos);
        mouseDir = new Vector2(mousepos.x - transform.position.x, mousepos.y - transform.position.y);
    }

    private IEnumerator StaminaRegen()
    {
        while (true)
        {
            curst = (curst <= maxst - stgen) ? curst + stgen : maxst;
            stbar.value = curst / maxst;
            yield return new WaitForSeconds(stgeninterval);
        }
        
    }

    internal void StaminaChange(float change)
    {
        if(curst + change > maxst)
        {
            curst = maxst;
        }
        else if(curst + change < 0f)
        {
            curst = 0f;
        }
        else
        {
            curst += change;
        }
        stbar.value = curst / maxst;
    }

    public void Flip()
    {
        if(mouseDir.x < 0)
        {
            sprite.flipX = true;
        }
        else
        {
            sprite.flipX = false;
        }
    }

    public IEnumerator Dodge(Vector2 dir)
    {
        canmove = false;
        canattack = false;
        candodge = false;
        isdodge = true;
        if(chargecoroutine != null)
        {
            StopCoroutine(chargecoroutine);

            chargecoroutine = null;
        }
        if (attackcoroutine != null)
        {
            StopCoroutine(attackcoroutine);
            attackcoroutine = null;
        }
        StaminaChange(-dodgest);
        rigid.bodyType = RigidbodyType2D.Kinematic;
        rigid.velocity = Vector2.zero;
        rigid.velocity = dir * dodgepower;
        invincibletime = (invincibletime > dodgetime) ? invincibletime : dodgetime;
        yield return new WaitForSeconds(dodgetime);
        canmove = true;
        canattack = true;
        isdodge = false;
        if (curst >= chargest)
        {
            cancharge = true;
        }
        if (curst >= dodgest)
        {
            candodge = true;
        }
        rigid.bodyType = RigidbodyType2D.Dynamic;
    }
    //////////////////////////////////   Attack   /////////////////////////
    internal void Attack(Vector2 dir,GameObject wpobj, AttackWeapon wp)
    {
        switch (wp.WeaponID())
        {
            case 0:
                attackcoroutine = StartCoroutine(playerattack.Longsword(dir, wpobj, wp));
                break;
            case 1:
                attackcoroutine = StartCoroutine(playerattack.Dagger(wpobj, wp));
                break;
            case 2:
                attackcoroutine = StartCoroutine(playerattack.Sword(dir, wpobj, wp, swordpowerchargetime));
                break;
            case 3:
                attackcoroutine = StartCoroutine(playerattack.Staff(dir, wpobj, wp));
                break;
        }
        swordpowerchargetime = 0f;
        swordpowercharging = false;
    }
    //////////////////////////////////   Attack   /////////////////////////
    //////////////////////////////////   Charge   /////////////////////////
    internal void Charge(Vector2 dir, GameObject wpobj, ChargeWeapon wp)
    {
        switch (wp.WeaponID())
        {
            case 0:
                chargecoroutine = StartCoroutine(playercharge.Longsword(dir, wpobj, wp));
                break;
            case 1:
                chargecoroutine = StartCoroutine(playercharge.Dagger(dir, wpobj, wp));
                break;
            case 2:
                chargecoroutine = StartCoroutine(playercharge.Sword(dir, wpobj, wp, swordpowerchargetime));
                break;
            case 3:
                chargecoroutine = StartCoroutine(playercharge.Staff(dir, wpobj, wp, mouseDir.magnitude));
                break;
        }
        swordpowerchargetime = 0f;
        swordpowercharging = false;
    }
    //////////////////////////////////   Charge   /////////////////////////

    void OnCollisionStay2D(Collision2D coll)
    {
        if (!invincible)
        {
            if(coll.gameObject.CompareTag("Enemy"))//Damage
            {
                Enemy enemy = coll.gameObject.GetComponent<Enemy>();
                if(enemy != null)
                {
                    Vector2 dir = coll.transform.position - this.transform.position;
                    Damage(enemy.Attack(), dir.normalized, enemy.KnockbackPower(),enemy.KnockbackTime());
                }
            }
        }

        else
        {
            return;
        }
    }
    public IEnumerator KnockBack(Vector2 dir, float knockback, float knockbacktime)
    {
        if (knockbackable)
        {
            canmove = false;
            isknockback = true;
            rigid.AddForce(dir * knockback, ForceMode2D.Impulse);

            yield return new WaitForSeconds(knockbacktime);

            isknockback = false;
            canmove = true;
        }
    }

    public void Damage(float damage, Vector2 dir, float knockbackpower, float knockbacktime)
    {
        if (!invincible)
        {
            curhp = (curhp - damage <= 0f) ? 0f : curhp - damage;
            hpbar.value = curhp / maxhp;
            if (curhp <= 0f)
            {
                Die();
            }
            else
            {
                invincibletime = (invincibletime > damagedelay) ? invincibletime : damagedelay;
            }
        }
    }

    public void Die()
    {
        Debug.Log("Dead");
    }
    public void SetMainWeapon(int id)
    {
        if(mainchargeobject == null)
        {
            mainchargeobject = Instantiate(chargeprefabs[id],transform);
            mainchargeobject.transform.parent = this.transform;
            maincharge = mainchargeobject.GetComponent<ChargeWeapon>();
            mainchargeobject.SetActive(true);
            mainchargeobject.SetActive(false);
            weaponmanager.maincharge_maxcooldown = maincharge.Delay();
        }
        if (mainattackobject == null)
        {
            mainattackobject = Instantiate(attackprefabs[id], transform);
            mainattackobject.transform.parent = this.transform;
            mainattack = mainattackobject.GetComponent<AttackWeapon>();
            mainattackobject.SetActive(true);
            mainattackobject.SetActive(false);
            weaponmanager.mainattack_maxcooldown = mainattack.Delay();
        }
    }
    public void SetSubWeapon(int id)
    {
        if (subchargeobject == null)
        {
            subchargeobject = Instantiate(chargeprefabs[id], transform);
            subchargeobject.transform.parent = this.transform;
            subcharge = subchargeobject.GetComponent<ChargeWeapon>();
            subchargeobject.SetActive(true);
            subchargeobject.SetActive(false);
            weaponmanager.subcharge_maxcooldown = subcharge.Delay();
        }
        if (subattackobject == null)
        {
            subattackobject = Instantiate(attackprefabs[id], transform);
            subattackobject.transform.parent = this.transform;
            subattack = subattackobject.GetComponent<AttackWeapon>();
            subattackobject.SetActive(true);
            subattackobject.SetActive(false);
            weaponmanager.subattack_maxcooldown = subattack.Delay();
        }
    }

    public bool Track()
    {
        return trackable;
    }

    public void Slow(float degree, float time, int spellid)
    {
        if (!SpellOnEffect.Contains(spellid))
        {
            StartCoroutine(SlowForSeconds(degree, time, spellid));
        }
        
    }
    protected IEnumerator SlowForSeconds(float degree, float time, int spellid)
    {
        SpellOnEffect.Add(spellid);
        speed_multiplier *= (1 - degree);
        yield return new WaitForSeconds(time);
        speed_multiplier /= (1 - degree);
        SpellOnEffect.Remove(spellid);
    }
}
