using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShopRoom : DungeonRoom
{
    internal override void Awake()
    {
        roomActivate = new UnityEvent();
        notActivated = false;
        roomType = 2;
    }

}
