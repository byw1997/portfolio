using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    Transform PlayerTransform;
    Vector3 Offset;
    // Start is called before the first frame update
    void Awake()
    {
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        Offset = transform.position - PlayerTransform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = PlayerTransform.position + Offset;
    }
}
