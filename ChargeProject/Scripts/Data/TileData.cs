using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Tile", menuName = "Scriptable Object/TileData")]
public class TileData : ScriptableObject
{
    public List<GameObject> scene1 = new List<GameObject>();
    public TileBase corridor1;
    public List<GameObject> scene2 = new List<GameObject>();
    public TileBase corridor2;
    public List<GameObject> scene3 = new List<GameObject>();
    public TileBase corridor3;
    public List<GameObject> scene4 = new List<GameObject>();
    public TileBase corridor4;
    public List<GameObject> scene5 = new List<GameObject>();
    public TileBase corridor5;
    public List<GameObject> scene6 = new List<GameObject>();
    public TileBase corridor6;
}
