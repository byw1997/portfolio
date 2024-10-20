using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlayerCharge : MonoBehaviour
{
    private Player player;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player> ();
    }
    public IEnumerator Longsword(Vector2 dir, GameObject wpobj, ChargeWeapon wp)
    {
        Debug.Log("Longsword Charge");
        wp.chargedir = dir;
        //player.rigid.bodyType = RigidbodyType2D.Kinematic;
        gameObject.layer = LayerMask.NameToLayer("PlayerOnCharge");
        wpobj.SetActive(true);
        player.canmove = false;
        player.cancharge = false;
        player.canattack = false;
        player.candodge = false;
        player.StaminaChange(-player.chargest);
        player.ischarge = true;
        player.rigid.velocity = Vector2.zero;
        player.rigid.velocity = dir * wp.ChargePower();
        player.trail.time = wp.ChargeTime();
        player.trail.emitting = true;
        yield return new WaitForSeconds(wp.ChargeTime());
        player.rigid.velocity = Vector2.zero;
        player.trail.emitting = false;
        player.ischarge = false;
        if (player.curst >= player.dodgest)
        {
            player.candodge = true;
        }
        wpobj.SetActive(false);
        //player.rigid.bodyType = RigidbodyType2D.Dynamic;
        gameObject.layer = LayerMask.NameToLayer("Player");
        yield return new WaitForSeconds(wp.Delay());
        player.canmove = true;
        player.canattack = true;
        if (player.curst >= player.chargest)
        {
            player.cancharge = true;
        }
    }
    public IEnumerator Dagger(Vector2 dir, GameObject wpobj, ChargeWeapon wp)
    {
        Debug.Log("Dagger Charge");
        wp.chargedir = dir;
        //player.rigid.bodyType = RigidbodyType2D.Kinematic;
        gameObject.layer = LayerMask.NameToLayer("PlayerOnCharge");
        wpobj.SetActive(true);
        player.canmove = false;
        player.cancharge = false;
        player.canattack = false;
        player.candodge = false;
        player.StaminaChange(-player.chargest);
        player.ischarge = true;
        player.rigid.velocity = Vector2.zero;
        player.trail.time = wp.ChargeTime();
        player.trail.emitting = true;
        float elapsedtime = 0f;
        while (wpobj.activeSelf && (elapsedtime < wp.ChargeTime()))
        {
            elapsedtime += Time.deltaTime;
            player.rigid.velocity = dir * wp.ChargePower();
            yield return null;
        }
        player.rigid.velocity = Vector2.zero;
        player.trail.emitting = false;
        player.ischarge = false;
        if (player.curst >= player.dodgest)
        {
            player.candodge = true;
        }
        wpobj.SetActive(false);
        if(elapsedtime < wp.ChargeTime())
        {
            player.rigid.AddForce(dir * -15f, ForceMode2D.Impulse);
        }
        yield return new WaitForSeconds(wp.Delay() * 0.8f);
        //player.rigid.bodyType = RigidbodyType2D.Dynamic;
        gameObject.layer = LayerMask.NameToLayer("Player");
        yield return new WaitForSeconds(wp.Delay() * 0.2f);
        player.canmove = true;
        player.canattack = true;
        if (player.curst >= player.chargest)
        {
            player.cancharge = true;
        }
    }

    public IEnumerator Sword(Vector2 dir, GameObject wpobj, ChargeWeapon wp, float powercharge)
    {
        Debug.Log("Sword Charge");
        wp.chargedir = dir;
        //player.rigid.bodyType = RigidbodyType2D.Kinematic;
        gameObject.layer = LayerMask.NameToLayer("PlayerOnCharge");
        wp.damage_multiplier = 1f + powercharge;
        wp.knockbackpower_multiplier = 1f + powercharge;
        wpobj.SetActive(true);
        player.canmove = false;
        player.cancharge = false;
        player.canattack = false;
        player.candodge = false;
        player.StaminaChange(-player.chargest);
        player.ischarge = true;
        player.rigid.velocity = Vector2.zero;
        player.trail.time = wp.ChargeTime();
        player.trail.emitting = true;
        player.rigid.velocity = dir * wp.ChargePower();
        yield return new WaitForSeconds(wp.ChargeTime());
        player.rigid.velocity = Vector2.zero;
        player.trail.emitting = false;
        player.ischarge = false;
        if (player.curst >= player.dodgest)
        {
            player.candodge = true;
        }
        wpobj.SetActive(false);
        wp.damage_multiplier = 1f;
        wp.knockbackpower_multiplier = 1f;
        yield return new WaitForSeconds(wp.Delay() * 0.8f);
        //player.rigid.bodyType = RigidbodyType2D.Dynamic;
        gameObject.layer = LayerMask.NameToLayer("Player");
        yield return new WaitForSeconds(wp.Delay() * 0.2f);
        player.canmove = true;
        player.canattack = true;
        if (player.curst >= player.chargest)
        {
            player.cancharge = true;
        }
    }

    public IEnumerator Staff(Vector2 dir, GameObject wpobj, ChargeWeapon wp, float magnitude)
    {
        player.canmove = false;
        player.cancharge = false;
        player.canattack = false;
        player.candodge = false;
        player.StaminaChange(-player.chargest);
        player.ischarge = true;
        player.rigid.velocity = Vector2.zero;

        wpobj.SetActive(true);
        yield return new WaitForSeconds(wp.Delay());
        float distance = (magnitude > wp.ChargePower())? wp.ChargePower() : magnitude;

        RaycastHit2D hitInfo = Physics2D.Raycast(player.rigid.position, dir, distance, LayerMask.GetMask("Wall"));

        if (hitInfo.collider == null)
        {
            player.rigid.position += dir * distance;
        }
        else
        {
            player.rigid.position = hitInfo.point;
        }

        player.rigid.velocity = Vector2.zero;
        player.ischarge = false;
        if (player.curst >= player.dodgest)
        {
            player.candodge = true;
        }
        player.canmove = true;
        player.canattack = true;
        if (player.curst >= player.chargest)
        {
            player.cancharge = true;
        }
    }
}
