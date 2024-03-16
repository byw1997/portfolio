using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Item : MonoBehaviour
{
    public float rotatespeed;
    
    void Update()
    {
        transform.Rotate(Vector3.up * rotatespeed * Time.deltaTime, Space.World);
    }

    
}
