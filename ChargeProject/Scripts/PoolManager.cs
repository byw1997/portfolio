using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    List<GameObject>[] pools;

    public GameObject[] prefabs;

    void Awake()
    {
        pools = new List<GameObject>[prefabs.Length];

        for(int i = 0; i < pools.Length; i++)
        {
            pools[i] = new List<GameObject>();
        }
    }

    public GameObject Get(int i)
    {
        GameObject temp = null;
        foreach(GameObject item in pools[i])
        {
            if (!item.activeSelf)
            {
                temp = item;
                temp.SetActive(true);
                break;
            }
        }
        if (!temp)
        {
            temp = Instantiate(prefabs[i], transform);
            pools[i].Add(temp);
        }
        return temp;
    }
}
