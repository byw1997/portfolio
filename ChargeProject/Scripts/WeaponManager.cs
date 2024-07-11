using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

using Debug = UnityEngine.Debug;
public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            mainattackrect = mainattack.GetComponent<RectTransform>();
            mainchargerect = maincharge.GetComponent<RectTransform>();
            subattackrect = subattack.GetComponent<RectTransform>();
            subchargerect = subcharge.GetComponent<RectTransform>();
            mainattackrectdisabled = mainattackdisabled.GetComponent<RectTransform>();
            mainchargerectdisabled = mainchargedisabled.GetComponent<RectTransform>();
            subattackrectdisabled = subattackdisabled.GetComponent<RectTransform>();
            subchargerectdisabled = subchargedisabled.GetComponent<RectTransform>();
            mainattack_available = true;
            maincharge_available = true;
            subattack_available = true;
            subcharge_available = true;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private Image mainattack;
    RectTransform mainattackrect;
    [SerializeField] private Image mainattackdisabled;
    RectTransform mainattackrectdisabled;

    [SerializeField] private Image maincharge;
    RectTransform mainchargerect;
    [SerializeField] private Image mainchargedisabled;
    RectTransform mainchargerectdisabled;

    [SerializeField] private Image subattack;
    RectTransform subattackrect;
    [SerializeField] private Image subattackdisabled;
    RectTransform subattackrectdisabled;

    [SerializeField] private Image subcharge;
    RectTransform subchargerect;
    [SerializeField] private Image subchargedisabled;
    RectTransform subchargerectdisabled;

    public float mainattack_maxcooldown;
    public float mainattack_cooldown;
    public bool mainattack_available;

    public float maincharge_maxcooldown;
    public float maincharge_cooldown;
    public bool maincharge_available;

    public float subattack_maxcooldown;
    public float subattack_cooldown;
    public bool subattack_available;

    public float subcharge_maxcooldown;
    public float subcharge_cooldown;
    public bool subcharge_available;

    void Swap(RectTransform a, RectTransform b)
    {
        Vector3 temp = a.localPosition;
        a.localPosition = b.localPosition;
        b.localPosition = temp;

        Vector2 temp2 = a.sizeDelta;
        a.sizeDelta = b.sizeDelta;
        b.sizeDelta = temp2;
    }
    public void SwapPosition()
    {
        Swap(mainattackrect, subattackrect);
        Swap(mainchargerect, subchargerect);
        mainattackrectdisabled.sizeDelta = mainattackrect.sizeDelta;
        mainchargerectdisabled.sizeDelta = mainchargerect.sizeDelta;
        subattackrectdisabled.sizeDelta = subattackrect.sizeDelta;
        subchargerectdisabled.sizeDelta = subchargerect.sizeDelta;
    }
    public void MainAttack()
    {
        StartCoroutine(ActivateMainAttack());
    }
    public IEnumerator ActivateMainAttack()
    {
        mainattack_cooldown = mainattack_maxcooldown;
        mainattack_available = false;
        while (mainattack_cooldown > 0f)
        {
            mainattack_cooldown -= Time.deltaTime;
            mainattackdisabled.fillAmount = mainattack_cooldown / mainattack_maxcooldown;
            yield return null;
        }
        mainattack_available = true;
    }
    public void MainCharge()
    {
        StartCoroutine(ActivateMainCharge());
    }
    public IEnumerator ActivateMainCharge()
    {
        maincharge_cooldown = maincharge_maxcooldown;
        maincharge_available = false;
        while (maincharge_cooldown > 0f)
        {
            maincharge_cooldown -= Time.deltaTime;
            mainchargedisabled.fillAmount = maincharge_cooldown / maincharge_maxcooldown;
            yield return null;
        }
        maincharge_available = true;
    }
    public void SubAttack()
    {
        StartCoroutine(ActivateSubAttack());
    }
    public IEnumerator ActivateSubAttack()
    {
        subattack_cooldown = subattack_maxcooldown;
        subattack_available = false;
        while (subattack_cooldown > 0f)
        {
            subattack_cooldown -= Time.deltaTime;
            subattackdisabled.fillAmount = subattack_cooldown / subattack_maxcooldown;
            yield return null;
        }
        subattack_available = true;
    }
    public void SubCharge()
    {
        StartCoroutine(ActivateSubCharge());
    }
    public IEnumerator ActivateSubCharge()
    {
        subcharge_cooldown = subcharge_maxcooldown;
        subcharge_available = false;
        while (subcharge_cooldown > 0f)
        {
            subcharge_cooldown -= Time.deltaTime;
            subchargedisabled.fillAmount = subcharge_cooldown / subcharge_maxcooldown;
            yield return null;
        }
        subcharge_available = true;
    }
}
