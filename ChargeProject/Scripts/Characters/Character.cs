using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    internal Rigidbody2D rigid;

    protected SpriteRenderer sprite;

    protected Animator anim;
    public float maxhp;
    public float curhp;
    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    public Rigidbody2D GetRigid()
    {
        return rigid;
    }

    
}
