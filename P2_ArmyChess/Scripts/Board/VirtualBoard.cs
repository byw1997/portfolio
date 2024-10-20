using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualBoard : MonoBehaviour
{
    internal Square[,] squares;

    void Create(int size)
    {
        squares = new Square[size, size];
    }
}
