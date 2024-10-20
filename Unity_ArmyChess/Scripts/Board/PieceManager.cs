using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
    // Start is called before the first frame update
    internal List<GameObject> whitePawn = new List<GameObject>();
    internal List<GameObject> blackPawn = new List<GameObject>();

    internal List<GameObject> whiteKnight = new List<GameObject>();
    internal List<GameObject> blackKnight = new List<GameObject>();

    internal List<GameObject> whiteBishop = new List<GameObject>();
    internal List<GameObject> blackBishop = new List<GameObject>();

    internal List<GameObject> whiteRook = new List<GameObject>();
    internal List<GameObject> blackRook = new List<GameObject>();

    internal List<GameObject> whiteQueen = new List<GameObject>();
    internal List<GameObject> blackQueen = new List<GameObject>();

    internal List<GameObject> whiteKing = new List<GameObject>();
    internal List<GameObject> blackKing = new List<GameObject>();

    public int whiteMovable;
    public int blackMovable;

    internal int[] whiteKingPos = new int[2];
    internal int[] blackKingPos = new int[2];

    public List<List<GameObject>> whitePiece;
    public List<List<GameObject>> blackPiece;

    void Awake()
    {
        whitePiece = new List<List<GameObject>>
        {
            whitePawn,
            whiteKnight,
            whiteBishop,
            whiteRook,
            whiteQueen,
            whiteKing
        };
        blackPiece = new List<List<GameObject>>
        {
            blackPawn,
            blackKnight,
            blackBishop,
            blackRook,
            blackQueen,
            blackKing
        };

    }

    internal void CleanMovableNumber()
    {
        whiteMovable = 0;
        blackMovable = 0;
    }

    internal void Clean()
    {
        CleanMovableNumber();
        foreach(var list in whitePiece)
        {
            list.Clear();
        }
        foreach (var list in blackPiece)
        {
            list.Clear();
        }
    }

    internal void Reset()
    {
        foreach(var pieces in whitePiece)
        {
            pieces.Clear();
        }
        foreach(var pieces in blackPiece)
        {
            pieces.Clear();
        }
    }
}
