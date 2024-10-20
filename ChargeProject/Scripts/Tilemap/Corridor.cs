using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Tilemaps;
using Debug = UnityEngine.Debug;

public class Corridor : MonoBehaviour
{
    DungeonRoom currentRoom;

    public List<Vector3Int> corridorPoints;

    public GameObject lockVertical;
    List<GameObject> vLocks;

    public GameObject lockHorizontal;
    List<GameObject> hLocks;

    public float halfLockWidth;

    int horizontal = 0;
    int vertical = 1;

    void Awake()
    {
        corridorPoints = new List<Vector3Int>();
        vLocks = new List<GameObject>();
        hLocks = new List<GameObject>();
        DontDestroyOnLoad(gameObject);
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Player")){
            Player player = coll.GetComponent<Player>();
            if(player != null)
            {
                if(player.currentRoom != null && player.currentRoom.notActivated)
                {
                    currentRoom = player.currentRoom;
                    Lock(currentRoom.transform.position, currentRoom.t);
                    player.currentRoom.Activate();
                }
            }
        }
    }

    void Lock(Vector3 roomPosition, Tilemap t)
    {
        Debug.Log("Lock called");
        Vector3Int intPosition = new Vector3Int((int)roomPosition.x, (int)roomPosition.y ,0);
        Debug.Log("Room is on " +  intPosition);
        Vector3Int up = new Vector3Int(0, 1, 0);
        Vector3Int down = new Vector3Int(0, -1, 0);
        Vector3Int right = new Vector3Int(1, 0, 0);
        Vector3Int left = new Vector3Int(-1, 0, 0);

        foreach (Vector3Int point in  corridorPoints)
        {
            if(t.GetTile(point - intPosition) == null)
            {
                if(t.GetTile(point - intPosition + up) != null)
                {
                    Debug.Log("Wall is on " + (point + up) + " and horizontal");
                    BlockCorridor(point + up + new Vector3(0.5f, -halfLockWidth, 0), horizontal);
                }
                else if(t.GetTile(point - intPosition + down) != null)
                {
                    Debug.Log("Wall is on " + (point) + " and horizontal");
                    BlockCorridor(point + new Vector3(0.5f, halfLockWidth, 0), horizontal);
                }
                else if(t.GetTile(point - intPosition + right) != null)
                {
                    Debug.Log("Wall is on " + (point + right) + " and vertical");
                    BlockCorridor(point + right + new Vector3(-halfLockWidth, 0.5f, 0), vertical);
                }
                else if(t.GetTile(point - intPosition + left) != null)
                {
                    Debug.Log("Wall is on " + (point) + " and vertical");
                    BlockCorridor(point + new Vector3(halfLockWidth, 0.5f, 0), vertical);
                }
            }
        }
    }

    void BlockCorridor(Vector3 pos, int direction)
    {
        if(direction == 0)
        {
            BlockHorizontal(pos);
        }
        else if (direction == 1)
        {
            BlockVertical(pos);
        }
        
    }

    void BlockHorizontal(Vector3 pos)
    {
        GameObject temp = null;
        foreach (GameObject item in hLocks)
        {
            if (!item.activeSelf)
            {
                temp = item;
                temp.transform.position = pos;
                temp.SetActive(true);
                break;
            }
        }
        if (!temp)
        {
            temp = Instantiate(lockHorizontal, pos, Quaternion.identity);
            hLocks.Add(temp);
        }
    }

    void BlockVertical(Vector3 pos)
    {
        GameObject temp = null;
        foreach (GameObject item in vLocks)
        {
            if (!item.activeSelf)
            {
                temp = item;
                temp.transform.position = pos;
                temp.SetActive(true);
                break;
            }
        }
        if (!temp)
        {
            temp = Instantiate(lockVertical, pos, Quaternion.identity);
            vLocks.Add(temp);
        }
    }

    public void Unlock()
    {
        foreach(GameObject item in hLocks)
        {
            item.SetActive(false);
        }
        foreach (GameObject item in vLocks)
        {
            item.SetActive(false);
        }
        Debug.Log("Cleared.");
    }
}
