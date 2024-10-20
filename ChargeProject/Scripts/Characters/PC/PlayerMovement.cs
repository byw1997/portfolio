using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Player player;

    void Start()
    {
        player = GetComponent<Player>();
    }

    internal void Move()
    {
        if (!player.canmove || player.isdodge || player.ischarge || player.isattack)
        {
            return;
        }
        if (!player.isdodge || !player.ischarge)
        {
            player.rigid.velocity = Vector2.zero;
        }
        player.speed = player.basespeed * player.speed_multiplier;
        Vector2 vec = player.input_vector.normalized * player.speed * Time.fixedDeltaTime;
        player.rigid.MovePosition(player.rigid.position + vec);
    }
}
