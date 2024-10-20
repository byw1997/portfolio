using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using Debug = UnityEngine.Debug;

public class Spawner : MonoBehaviour
{
    public UnityEvent waveEnd;
    public UnityEvent stageClear;

    public List<EnemyWave> wave;
    public EnemyWave bossWave;
    List<int> enemies;
    int numberOfWave;
    int enemyAlive;

    int currentGen;
    int minGen = 2;
    int maxGen = 4;

    void Awake()
    {
        waveEnd = new UnityEvent();
        stageClear = new UnityEvent();
        DontDestroyOnLoad(gameObject);
    }

    public void CallSpawn()
    {
        Debug.Log("CallSpawn() called");
        StartCoroutine(Spawn(GameManager.instance.player.currentRoom));
    }
    public IEnumerator Spawn(DungeonRoom room)
    {
        Tilemap t = room.GetComponent<Tilemap>();
        if(room.roomType == 1)
        {
            numberOfWave = UnityEngine.Random.Range(minGen, maxGen + 1);
            while (numberOfWave > 0)
            {
                enemies = wave[UnityEngine.Random.Range(0, wave.Count)].enemyList;
                currentGen = 0;
                while (currentGen < enemies.Count)
                {
                    GameManager.instance.pool.Get(t, enemies[currentGen++]);
                    enemyAlive++;
                    yield return new WaitForSeconds(0.5f);
                }
                numberOfWave--;//Get Enemy alive
                while (enemyAlive > 0)
                {
                    yield return null;
                }
            }
            if (waveEnd != null)
            {
                waveEnd.Invoke();
            }
        }
        else
        {
            enemies = bossWave.enemyList;
            currentGen = 0;
            while (currentGen < enemies.Count)
            {
                GameManager.instance.pool.Get(t, enemies[currentGen++]);
                enemyAlive++;
                yield return new WaitForSeconds(0.5f);
            }
            while (enemyAlive > 0)
            {
                yield return null;
            }
            if (waveEnd != null)
            {
                waveEnd.Invoke();
                stageClear.Invoke();
            }
        }
        
        
    }
    public void EnemyDead()
    {
        enemyAlive--;
    }
}
