using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(spawn());
    }
    public IEnumerator spawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            GameManager.instance.pool.Get(0);
            yield return new WaitForSeconds(3f);
            GameManager.instance.pool.Get(1);
        }
        
    }
}
