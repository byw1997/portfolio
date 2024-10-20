using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

public class DungeonRoom : MonoBehaviour
{
    public UnityEvent roomActivate;
    public bool notActivated;
    public Tilemap t;
    //int spawnTime;
    public int roomType;
    public Vector3 pos;

    internal virtual void Awake()
    {
        roomActivate = new UnityEvent();
        notActivated = true;
        roomType = 1;
        pos = transform.position;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            Debug.Log(gameObject.name + " entered");
            Player player = coll.GetComponent<Player>();
            if (player != null)
            {
                player.currentRoom = this;
            }
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            Debug.Log(gameObject.name + " exited");
            Player player = coll.GetComponent<Player>();
            if (player != null)
            {
                player.currentRoom = null;
            }
        }
    }
    public void Activate()
    {
        notActivated = false;
        if(roomActivate != null)
        {
            roomActivate.Invoke();
        }
    }
}
