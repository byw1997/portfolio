using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class AttackStaff : AttackWeapon
{
    List<GameObject> bullets;

    public GameObject bulletprefab;

    protected override void Awake()
    {
        base.Awake();

        weaponid = 3;
        baseknockbackpower = 10f;
        baseknockbacktime = 0.1f;
        basedamage = 20f;
        basedelay = 0.2f;
        initialrotation = 0f;
        lengthdivider = 0f;
        speed = 15f;
        attacktime = 0.4f;

        bullets = new List<GameObject>();
    }

    void OnEnable()
    {
        knockbackpower = baseknockbackpower * knockbackpower_multiplier;
        knockbacktime = baseknockbacktime * knockbacktime_multiplier;
        damage = basedamage * damage_multiplier;
        delay = basedelay * delay_multiplier;
        GetBullet();
    }

    public GameObject GetBullet()
    {
        GameObject temp = null;
        foreach (GameObject item in bullets)
        {
            if (!item.activeSelf)
            {
                temp = item;

                break;
            }
        }
        if (!temp)
        {
            temp = Instantiate(bulletprefab, transform);
            temp.SetActive(true);
            temp.transform.parent = this.transform;
            temp.SetActive(false);
            bullets.Add(temp);
        }
        temp.GetComponent<StaffBullet>().SetSpec(damage, speed, attacktime, targetdir, knockbackpower, knockbacktime);
        temp.SetActive(true);
        Debug.Log("Staff target dir = " + targetdir);

        return temp;
    }
}
