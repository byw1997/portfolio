using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TestWeaponChanger : MonoBehaviour
{
    public int weaponid;
    public GameObject notification;
    private TMP_Text notification_text;
    private Coroutine textcoroutine;
    private string temp;

    void Awake()
    {
        notification_text = notification.GetComponent<TMP_Text>();
        notification_text.color = Color.black;
        notification_text.fontSize = 40;
    }

    void OnSceneLoaded()
    {
        gameObject.SetActive(true);
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Player")){
            temp = "Your ";
            Player player = coll.GetComponent<Player>();
            if (!player.mainattackobject && !player.mainchargeobject)
            {
                temp += "main ";
                player.SetMainWeapon(weaponid);
            }
            else if(!player.subattackobject && !player.subchargeobject)
            {
                temp += "sub ";
                player.SetSubWeapon(weaponid);
            }
            else
            {
                return;
            }
            temp += "weapon is set to " + WeaponName(weaponid);
            notification_text.text = temp;
            if (textcoroutine != null)
            {
                StopCoroutine(textcoroutine);
            }
            textcoroutine = StartCoroutine(ShowText());
        }
    }

    string WeaponName(int id)
    {
        switch (id)
        {
            case 0:
                return "Longsword";
            case 1:
                return "Dagger";
            case 2:
                return "Sword";
            case 3:
                return "Staff";
            default:
                return "Unknown";

        }
    }
    IEnumerator ShowText()
    {
        notification.SetActive(true);
        yield return new WaitForSeconds(3);
        notification.SetActive(false);
    }
}
