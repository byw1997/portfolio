using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
public class PlayerInput : MonoBehaviour
{
    private Player player;

    void Start()
    {
        player = GetComponent<Player>();
    }

    internal void GetInput()
    {
        player.TraceCursor();
        Vector2 mouseDirnorm = player.mouseDir.normalized;
        player.invincibletime = (player.invincibletime > Time.deltaTime) ? player.invincibletime - Time.deltaTime : 0f;
        if (player.invincibletime > 0f)
        {
            player.invincible = true;
        }
        else
        {
            player.invincible = false;
        }
        if (Input.GetButtonDown("Swap"))
        {
            player.weaponmanager.SwapPosition();
        }
        if (Input.GetButtonUp("Swap"))
        {
            player.weaponmanager.SwapPosition();
        }
        if (!player.canmove || player.isdodge || player.ischarge || player.isattack || player.isknockback)
        {
            return;
        }
        if (Input.GetButtonDown("Dodge") && player.candodge)
        {
            player.StartCoroutine(player.Dodge(mouseDirnorm));
        }
        if (player.subattack && (player.subattack.WeaponID() == 2) && player.swordpowercharging && Input.GetButton("Swap") && Input.GetButton("Attack") && player.canattack && player.weaponmanager.subattack_available)
        {
            player.swordpowerchargetime = (player.swordpowerchargetime + Time.deltaTime > 3f) ? 3f : player.swordpowerchargetime + Time.deltaTime;
        }
        else if (player.subattack && (player.subattack.WeaponID() == 2) && player.swordpowercharging && (Input.GetButtonUp("Swap") || Input.GetButtonUp("Attack")) && player.canattack && player.weaponmanager.subattack_available)
        {
            player.weaponmanager.SubAttack();
            player.trackable = true;
            player.Attack(mouseDirnorm, player.subattackobject, player.subattack);
        }
        else if (player.subattack && Input.GetButton("Swap") && Input.GetButtonDown("Attack") && player.canattack && player.weaponmanager.subattack_available)
        {
            if (player.subattack.WeaponID() == 2)
            {
                player.swordpowercharging = true;
            }
            else
            {
                player.weaponmanager.SubAttack();
                player.trackable = true;
                player.Attack(mouseDirnorm, player.subattackobject, player.subattack);
            }
        }
        else if (player.mainattack && (player.mainattack.WeaponID() == 2) && player.swordpowercharging && Input.GetButton("Attack") && player.canattack && player.weaponmanager.mainattack_available)
        {
            player.swordpowerchargetime = (player.swordpowerchargetime + Time.deltaTime > player.maxswordpowerchargetime) ? player.maxswordpowerchargetime : player.swordpowerchargetime + Time.deltaTime;
        }
        else if (player.mainattack && (player.mainattack.WeaponID() == 2) && player.swordpowercharging && Input.GetButtonUp("Attack") && player.canattack && player.weaponmanager.mainattack_available)
        {
            player.weaponmanager.MainAttack();
            player.trackable = true;
            player.Attack(mouseDirnorm, player.mainattackobject, player.mainattack);
        }
        else if (player.mainattack && Input.GetButtonDown("Attack") && player.canattack && player.weaponmanager.mainattack_available)
        {
            if (player.mainattack.WeaponID() == 2)
            {
                player.swordpowercharging = true;
            }
            else
            {
                player.weaponmanager.MainAttack();
                player.trackable = true;
                player.Attack(mouseDirnorm, player.mainattackobject, player.mainattack);
            }
        }

        if (player.subcharge && (player.subcharge.WeaponID() == 2) && player.swordpowercharging && Input.GetButton("Swap") && Input.GetButton("Charge") && player.cancharge && player.weaponmanager.subcharge_available)
        {
            player.swordpowerchargetime = (player.swordpowerchargetime + Time.deltaTime > 3f) ? 3f : player.swordpowerchargetime + Time.deltaTime;
        }
        else if (player.subcharge && (player.subcharge.WeaponID() == 2) && player.swordpowercharging && (Input.GetButtonUp("Swap") || Input.GetButtonUp("Charge")) && player.cancharge && player.weaponmanager.subcharge_available)
        {
            player.weaponmanager.SubCharge();
            player.trackable = true;
            player.Charge(mouseDirnorm, player.subchargeobject, player.subcharge);
        }
        else if (player.subcharge && Input.GetButton("Swap") && Input.GetButtonDown("Charge") && player.cancharge && player.weaponmanager.subcharge_available)
        {
            if (player.subcharge.WeaponID() == 2)
            {
                player.swordpowercharging = true;
            }
            else
            {
                player.weaponmanager.SubCharge();
                player.trackable = true;
                player.Charge(mouseDirnorm, player.subchargeobject, player.subcharge);
            }
        }
        else if (player.maincharge && (player.maincharge.WeaponID() == 2) && player.swordpowercharging && Input.GetButton("Charge") && player.cancharge && player.weaponmanager.maincharge_available)
        {
            player.swordpowerchargetime = (player.swordpowerchargetime + Time.deltaTime > player.maxswordpowerchargetime) ? player.maxswordpowerchargetime : player.swordpowerchargetime + Time.deltaTime;
        }
        else if (player.maincharge && (player.maincharge.WeaponID() == 2) && player.swordpowercharging && Input.GetButtonUp("Charge") && player.cancharge && player.weaponmanager.maincharge_available)
        {
            player.weaponmanager.MainCharge();
            player.trackable = true;
            player.Charge(mouseDirnorm, player.mainchargeobject, player.maincharge);
        }
        else if (player.maincharge && Input.GetButtonDown("Charge") && player.cancharge && player.weaponmanager.maincharge_available)
        {
            if (player.maincharge.WeaponID() == 2)
            {
                player.swordpowercharging = true;
            }
            else
            {
                player.weaponmanager.MainCharge();
                player.trackable = true;
                player.Charge(mouseDirnorm, player.mainchargeobject, player.maincharge);
            }
        }
        if (player.canmove)
        {
            player.input_vector.x = Input.GetAxisRaw("Horizontal");
            player.input_vector.y = Input.GetAxisRaw("Vertical");
        }
    }
}
