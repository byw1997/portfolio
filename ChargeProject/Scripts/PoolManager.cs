using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;

public class PoolManager : MonoBehaviour
{
    List<GameObject>[] pools;

    public GameObject[] prefabs;

    public Spawner spawner;

    public GameObject genWarning;
    List<GameObject> warningPool;

    void Awake()
    {
        pools = new List<GameObject>[prefabs.Length];

        for(int i = 0; i < pools.Length; i++)
        {
            pools[i] = new List<GameObject>();
        }
        warningPool = new List<GameObject>();
        DontDestroyOnLoad(gameObject);
    }

    public void Get(Tilemap t, int i)
    {
        GameObject temp = null;
        StartCoroutine(Generate(temp, t, i));
        
        //return temp;
    }

    IEnumerator Generate(GameObject temp, Tilemap t, int i)
    {
        BoundsInt bound = t.cellBounds;
        TileBase tile = null;
        Vector3Int randomPosition = new Vector3Int(0, 0, 0);
        while (tile == null)
        {
            randomPosition.x = Random.Range(bound.xMin, bound.xMax);
            randomPosition.y = Random.Range(bound.yMin, bound.yMax);

            tile = t.GetTile(randomPosition);
        }

        foreach (GameObject item in warningPool)
        {
            if (!item.activeSelf)
            {
                temp = item;
                temp.transform.position = randomPosition + t.transform.position;
                temp.SetActive(true);
                break;
            }
        }
        if (!temp)
        {
            temp = Instantiate(genWarning, (Vector3)randomPosition + t.transform.position, Quaternion.identity, gameObject.transform);
            warningPool.Add(temp);
        }

        yield return new WaitForSeconds(1f);

        temp.SetActive(false);

        temp = null;

        foreach (GameObject item in pools[i])
        {
            if (!item.activeSelf)
            {
                temp = item;
                temp.transform.position = randomPosition + t.transform.position;
                temp.SetActive(true);
                break;
            }
        }
        if (!temp)
        {
            temp = Instantiate(prefabs[i], (Vector3)randomPosition + t.transform.position, Quaternion.identity);
            temp.GetComponent<Enemy>().enemyDead.AddListener(spawner.EnemyDead);
            pools[i].Add(temp);
        }
    }
}
