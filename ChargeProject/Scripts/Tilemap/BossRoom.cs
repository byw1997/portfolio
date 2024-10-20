using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossRoom : DungeonRoom
{
    public GameObject stairTile;
    public Vector3 stairPos;

    internal override void Awake()
    {
        roomActivate = new UnityEvent();
        notActivated = true;
        roomType = 3;
    }

    public void CreateStair()
    {
        stairPos = gameObject.transform.position + new Vector3(0.5f, 0.5f, 0);
        GameObject stair = Instantiate(stairTile, stairPos, Quaternion.identity);
    }
}
