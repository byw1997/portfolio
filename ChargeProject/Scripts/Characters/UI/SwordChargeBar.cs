using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordChargeBar : MonoBehaviour
{
    Rigidbody2D player;
    void Awake()
    {
        player = GameManager.instance.player.GetRigid();
    }
    // Start is called before the first frame update
    void OnEnable()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
