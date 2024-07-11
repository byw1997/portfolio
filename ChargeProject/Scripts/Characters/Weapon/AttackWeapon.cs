using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;


public class AttackWeapon : Weapon
{
    protected float length;
    protected Vector2 playerposition;
    protected float initialrotation;
    protected float lengthdivider;
    protected float speed;
    protected float attacktime;

    protected Vector2 targetdir;


    protected virtual void Awake()
    {
        length = transform.localScale.x;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }
    public void SetPosition(Vector2 dir, Vector2 pcpos)
    {
        Debug.Log("SetPosition" + this.name);
        float rotateinradian = initialrotation * Mathf.Deg2Rad;
        float rotatecos = Mathf.Cos(rotateinradian);
        float rotatesin = Mathf.Sin(rotateinradian);
        Vector2 rotateddir = new Vector2(dir.x * rotatecos -dir.y * rotatesin, dir.x * rotatesin + dir.y * rotatecos);
        transform.localPosition = rotateddir * length * lengthdivider;
        transform.LookAt(pcpos);
        playerposition =pcpos;
        Debug.Log("Player is in " + pcpos);
        Debug.Log(this.name+ " is in " + this.transform.position);
    }
    public void SetDir(Vector2 dir)
    {
        targetdir = dir;
    }
    public float AttackTime()
    {
        return attacktime;
    }
}
