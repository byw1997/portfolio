using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWave", menuName = "Scriptable Object/Enemy Wave Data")]
public class EnemyWave : ScriptableObject
{
    public List<int> enemyList = new List<int>();
}
