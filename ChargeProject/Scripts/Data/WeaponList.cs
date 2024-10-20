using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponList", menuName = "Scriptable Object/WeaponList")]
public class WeaponList : ScriptableObject
{
    public List<int> WeaponID = new List<int>();
    public List<string> WeaponName = new List<string>();
}
