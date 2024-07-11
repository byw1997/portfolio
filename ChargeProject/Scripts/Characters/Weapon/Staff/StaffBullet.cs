using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class StaffBullet : MonoBehaviour
{
    private Transform parent;

    public float damage;
    public float speed;
    public float knockbackpower;
    public float knockbacktime;
    public float attacktime;
    public Vector2 targetdir;
    public Rigidbody2D rigid;
    List<Collider2D> enemyhit;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        enemyhit = new List<Collider2D>();
    }

    void OnEnable()
    {
        Debug.Log("Bullet target dir = " + targetdir);
        this.parent = transform.parent;
        transform.parent = null;
        rigid.velocity = targetdir * speed;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(-targetdir.x, targetdir.y) * 180 / Mathf.PI));
        StartCoroutine(DisableSelf());
    }

    void OnDisable()
    {
        enemyhit.Clear();
        rigid.velocity = Vector2.zero;
    }

    public void SetSpec(float dmg, float spd, float atktime, Vector2 dir, float knockbackpw, float knockbacktm)
    {
        Debug.Log("SetSpec target dir = " + targetdir);
        damage = dmg; speed = spd; targetdir = dir; attacktime = atktime;
        knockbackpower = knockbackpw; knockbacktime = knockbacktm;
        Debug.Log("After SetSpec target dir = " + targetdir);
    }

    public IEnumerator DisableSelf()
    {
        yield return new WaitForSeconds(attacktime);
        transform.parent = this.parent;
        transform.localPosition = Vector2.zero;
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Enemy") && !enemyhit.Contains(coll))
        {
            IDamageable enemy = coll.GetComponent<IDamageable>();
            if (enemy != null)
            {
                Vector2 dir = coll.transform.position - this.transform.position;
                enemy.Damage(damage, dir.normalized, knockbackpower, knockbacktime);
                enemyhit.Add(coll);
            }
        }
    }
}
