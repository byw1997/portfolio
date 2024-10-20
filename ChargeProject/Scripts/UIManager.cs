using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
public class UIManager : MonoBehaviour
{
    [SerializeField] private Slider hpBar;
    [SerializeField] private Slider stBar;
    [SerializeField] private Slider swordChargeBar;
    private RectTransform chargeBarTransform;

    [SerializeField] private Player player;

    private Vector3 offset = new Vector3(0, 80, 0);

    void Awake()
    {
        hpBar.value = 1f;
        stBar.value = 0f;
        chargeBarTransform = swordChargeBar.GetComponent<RectTransform>();
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        hpBar.value = player.curhp / player.maxhp;
        stBar.value = player.curst / player.maxst;

        if (!swordChargeBar.gameObject.activeSelf && player.swordpowercharging)
        {
            swordChargeBar.gameObject.SetActive(true);
        }
        if (player.swordpowercharging)
        {
            
            swordChargeBar.value = player.swordpowerchargetime / player.maxswordpowerchargetime;
        }
        if(swordChargeBar.gameObject.activeSelf && !player.swordpowercharging)
        {
            swordChargeBar.gameObject.SetActive(false);
        }
    }
    void FixedUpdate()
    {
        chargeBarTransform.position = Camera.main.WorldToScreenPoint(player.transform.position) + offset;
    }
}
