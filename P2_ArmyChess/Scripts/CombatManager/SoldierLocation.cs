using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierLocation : MonoBehaviour
{
    public int locationId;
    public List<GameObject> sprites;
    GameObject activeObject = null;
    public GameObject ActiveObject
    {
        get { return activeObject; }
        private set { activeObject = value; }
    }
    public void ActivateSoldier(int id)
    {
        sprites[id].SetActive(true);
        activeObject = sprites[id];
    }
    public void DeactivateSoldier()
    {
        if (activeObject != null)
        {
            activeObject.SetActive(false);
            activeObject = null;
        }
    }

}
