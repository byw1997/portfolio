using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    public int x;
    public int y;
    public GameObject pieceOnSquare;
    public GameObject highlight;
    public bool highlighted = false;

    public float whiteAtkBonus = 0;
    public float blackAtkBonus = 0;
    public float whiteDefBonus = 0;
    public float blackDefBonus = 0;

    private void Awake()
    {
        pieceOnSquare = null;
    }

    public void Highlight()
    {
        highlight.SetActive(true);
        highlighted = true;
    }

    public void UnHighlight()
    {
        highlight.SetActive(false);
        highlighted = false;
    }

    public void MoveTo(Square target)
    {
        Piece piece = pieceOnSquare.GetComponent<Piece>();
        piece.MoveTo(target);
        target.pieceOnSquare = pieceOnSquare;
        pieceOnSquare = null;
        if(piece.pieceType == 5)
        {
            if(piece.team == 0)
            {
                GameManager.instance.board.pieceManager.whiteKingPos[0] = piece.x;
                GameManager.instance.board.pieceManager.whiteKingPos[1] = piece.y;
            }
            else
            {
                GameManager.instance.board.pieceManager.blackKingPos[0] = piece.x;
                GameManager.instance.board.pieceManager.blackKingPos[1] = piece.y;
            }
        }
    }

    public void CleanBonus()
    {
        whiteAtkBonus = 0;
        blackAtkBonus = 0;
        whiteDefBonus = 0;
        blackDefBonus = 0;
    }

    public string SquareInfo()
    {
        string temp = "(" + x + ", " + y + ")";
        temp += "\nwhite atk bonus: " + whiteAtkBonus + "\nwhite def bonus: " + whiteDefBonus + "\nblack atk bonus: " + blackAtkBonus + "\nblack def bonus: " + blackDefBonus;
        return temp;
    }

    public void Clean()
    {
        CleanBonus();
        UnHighlight();
        pieceOnSquare = null;
    }

}
