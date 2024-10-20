using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEditor.Search;
using UnityEngine;

public class PiecePlacer : MonoBehaviour
{

    [SerializeField] List<GameObject> whitePrefab;

    [SerializeField] List<GameObject> blackPrefab;
    List<List<GameObject>> prefab;

    List<List<GameObject>> whitePool = new List<List<GameObject>>();
    List<List<GameObject>> blackPool = new List<List<GameObject>>();
    List<List<List<GameObject>>> pool = new List<List<List<GameObject>>>();

    Vector3 norm = new Vector3(0, 0.5f, 0);

    private void Awake()
    {
        prefab = new List<List<GameObject>>();
        prefab.Add(whitePrefab);
        prefab.Add(blackPrefab);
        for (int i = 0; i < 6; i++)
        {
            whitePool.Add(new List<GameObject>());
        }
        for (int i = 0; i < 6; i++)
        {
            blackPool.Add(new List<GameObject>());
        }
        pool.Add(whitePool);
        pool.Add(blackPool);
    }

    internal GameObject Place(int team, int pieceType, GameObject square)
    {
        GameObject temp = null;
        foreach(GameObject item in pool[team][pieceType])
        {
            if (item.activeSelf == false)
            {
                temp = item;
                temp.transform.position = square.transform.position + norm;
                temp.SetActive(true);
                break;
            }
        }
        if(temp == null)
        {
            temp = Instantiate(prefab[team][pieceType], square.transform.position + norm, Quaternion.Euler(-90, 90, 0), gameObject.transform);
        }
        pool[team][pieceType].Add(temp);
        return temp;
    }

    internal void Clean()
    {
        foreach(var list in whitePool)
        {
            foreach(GameObject item in list)
            {
                item.GetComponent<Piece>().Clean();
                item.SetActive(false);
            }
        }
        foreach (var list in blackPool)
        {
            foreach (GameObject item in list)
            {
                item.GetComponent<Piece>().Clean();
                item.SetActive(false);
            }
        }
    }

}
