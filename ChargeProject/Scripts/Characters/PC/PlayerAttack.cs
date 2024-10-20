using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlayerAttack : MonoBehaviour
{
    private Player player;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
    }
    public IEnumerator Longsword(Vector2 dir, GameObject wpobj, AttackWeapon wp)
    {
        Debug.Log("Longsword Attack");
        player.canmove = false;
        player.cancharge = false;
        player.canattack = false;
        player.candodge = false;
        player.isattack = true;
        player.rigid.bodyType = RigidbodyType2D.Kinematic;
        player.rigid.velocity = Vector2.zero;
        wp.SetPosition(dir, transform.position);
        wpobj.SetActive(true);
        yield return new WaitForSeconds(wp.AttackTime());
        wpobj.SetActive(false);
        player.rigid.velocity = Vector2.zero;
        player.rigid.bodyType = RigidbodyType2D.Dynamic;
        if (player.curst >= player.dodgest)
        {
            player.candodge = true;
        }
        yield return new WaitForSeconds(wp.Delay());
        player.canmove = true;
        player.canattack = true;
        if (player.curst >= player.chargest)
        {
            player.cancharge = true;
        }
        player.isattack = false;
    }

    public IEnumerator Dagger(GameObject wpobj, AttackWeapon wp)
    {
        Debug.Log("Dagger Attack");
        player.trackable = false;
        player.speed_multiplier *= 2f;
        float elapsedtime = 0f;
        while(wp.AttackTime() > elapsedtime)
        {
            elapsedtime += Time.deltaTime;
            player.StaminaChange(25f * Time.deltaTime);
            yield return null;
        }
        player.speed_multiplier /= 2f;
        player.trackable = true;
    }

    public IEnumerator Sword(Vector2 dir, GameObject wpobj, AttackWeapon wp, float powercharge)
    {
        Debug.Log("Sword Attack");
        player.canmove = false;
        player.cancharge = false;
        player.canattack = false;
        player.candodge = false;
        player.isattack = true;
        player.rigid.bodyType = RigidbodyType2D.Kinematic;
        player.rigid.velocity = Vector2.zero;
        wp.SetPosition(dir, transform.position);
        wp.SetDir(dir);
        wp.damage_multiplier = 1f + powercharge;
        wpobj.SetActive(true);
        yield return new WaitForSeconds(wp.AttackTime());
        wpobj.SetActive(false);
        wp.damage_multiplier = 1f;
        player.rigid.velocity = Vector2.zero;
        player.rigid.bodyType = RigidbodyType2D.Dynamic;
        if (player.curst >= player.dodgest)
        {
            player.candodge = true;
        }
        yield return new WaitForSeconds(wp.Delay());
        player.canmove = true;
        player.canattack = true;
        if (player.curst >= player.chargest)
        {
            player.cancharge = true;
        }
        player.isattack = false;
    }

    public IEnumerator Staff(Vector2 dir, GameObject wpobj, AttackWeapon wp)
    {
        Debug.Log("Staff Attack");
        player.canmove = false;
        player.cancharge = false;
        player.canattack = false;
        player.isattack = true;
        player.rigid.bodyType = RigidbodyType2D.Kinematic;
        player.rigid.velocity = Vector2.zero;
        wp.SetDir(dir);
        wpobj.SetActive(true);
        yield return new WaitForSeconds(wp.Delay());
        wpobj.SetActive(false);
        player.rigid.velocity = Vector2.zero;
        player.rigid.bodyType = RigidbodyType2D.Dynamic;
        player.canmove = true;
        player.canattack = true;
        if (player.curst >= player.chargest)
        {
            player.cancharge = true;
        }
        player.isattack = false;
    }
}

