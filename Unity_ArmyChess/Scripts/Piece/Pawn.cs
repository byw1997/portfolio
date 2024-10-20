using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    public bool twoSquareAdvance = false;
    void Awake()
    {
        pieceType = 0;
    }

    public override void Clean()
    {
        base.Clean();
        twoSquareAdvance = false;
    }
}
