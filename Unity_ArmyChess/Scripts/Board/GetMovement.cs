using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GetMovement : MonoBehaviour
{
    Board board;
    PieceManager pieceManager;
    int iter;
    List<int[]> temp;
    List<int[]> temp2;

    private void Awake()
    {
        board = GetComponent<Board>();
        pieceManager = GetComponent<PieceManager>();
    }

    internal bool IsChecking(int team, int x, int y, int pieceType)
    {
        List<int[]> ret = new List<int[]>();
        int[] enemyKingPos;
        if(team == 0)
        {
            enemyKingPos = pieceManager.blackKingPos;
        }
        else
        {
            enemyKingPos= pieceManager.whiteKingPos;
        }
        switch (pieceType)
        {
            case 0:
                ret.AddRange(GetPawnAttack(team, x, y));
                break;
            case 1:
                ret.AddRange(GetKnightMove(team, x, y));
                break;
            case 2:
                ret.AddRange(GetDiagonal(team, x, y));
                break;
            case 3:
                ret.AddRange(GetStraight(team, x, y));
                break;
            case 4:
                ret.AddRange(GetDiagonal(team, x, y));
                ret.AddRange(GetStraight(team, x, y));
                break;
            case 5:
                ret.AddRange(GetKingAttack(team, x, y));
                break;
        }
        foreach(var move in ret)
        {
            if (enemyKingPos.SequenceEqual(move))
            {
                return true;
            }
        }
        return false;
    }

    bool isCheckAfterMove(int team, Square source, int[] target)
    {
        bool ret = false;
        List<List<GameObject>> enemyPiece;
        if(team == 0)
        {
            enemyPiece = pieceManager.blackPiece;
        }
        else
        {
            enemyPiece = pieceManager.whitePiece;
        }
        Square targetSquare = board.squares[target[0], target[1]].GetComponent<Square>();
        GameObject lastTakenPiece = targetSquare.pieceOnSquare;
        Piece lastPiece = null;
        if(lastTakenPiece)
        {
            lastPiece = lastTakenPiece.GetComponent<Piece>();
            if(lastPiece.team != team)
            {
                enemyPiece[lastPiece.pieceType][lastPiece.id] = null;
            }
        }
        source.MoveTo(targetSquare);
        if (board.isCheck(team))
        {
            ret = true;
        }
        targetSquare.MoveTo(source);
        if (lastTakenPiece && lastPiece)
        {
            if(lastPiece.team != team)
            {
                enemyPiece[lastPiece.pieceType][lastPiece.id] = lastTakenPiece;
            }
            targetSquare.pieceOnSquare = lastTakenPiece;
        }
        return ret;
    }


    internal List<int[]> GetMove(int team, int x, int y, int pieceType, float atkSum, float defSum)//0 = pawn, 1 = knight, 2 = bishop, 3 = rook, 4 = queen, 5 = king
    {
        List<int[]> ret = new List<int[]>();
        List<int[]> ret2 = new List<int[]>();
        switch (pieceType)
        {
            case 0:
                ret.AddRange(GetPawnMove(team, x, y, atkSum, defSum).Item1);
                ret2.AddRange(GetPawnMove(team, x, y, atkSum, defSum).Item2);
                break;
            case 1:
                ret.AddRange(GetKnightMove(team, x, y, atkSum, defSum).Item1);
                ret2.AddRange(GetKnightMove(team, x, y, atkSum, defSum).Item2);
                break;
            case 2:
                ret.AddRange(GetDiagonal(team, x, y, atkSum, defSum).Item1);
                ret2.AddRange(GetDiagonal(team, x, y, atkSum, defSum).Item2);
                break;
            case 3:
                ret.AddRange(GetStraight(team, x, y, atkSum, defSum).Item1);
                ret2.AddRange(GetStraight(team, x, y, atkSum, defSum).Item2);
                break;
            case 4:
                ret.AddRange(GetDiagonal(team, x, y, atkSum, defSum).Item1);
                ret2.AddRange(GetDiagonal(team, x, y, atkSum, defSum).Item2);
                ret.AddRange(GetStraight(team, x, y, atkSum, defSum).Item1);
                ret2.AddRange(GetStraight(team, x, y, atkSum, defSum).Item2);
                break;
            case 5:
                ret.AddRange(GetKingMove(team, x, y));
                break;
        }
        Square source = board.squares[x, y].GetComponent<Square>();
        ret.RemoveAll(move => isCheckAfterMove(team, source, move));
        ret2.RemoveAll(move => isCheckAfterMove(team, source, move));
        if(team == 0)
        {
            pieceManager.whiteMovable += ret.Count();
            foreach(var move in ret2)
            {
                board.squares[move[0], move[1]].GetComponent<Square>().whiteAtkBonus += atkSum * GameManager.instance.bonusRate;
                board.squares[move[0], move[1]].GetComponent<Square>().whiteDefBonus += defSum * GameManager.instance.bonusRate;
            }
        }
        else
        {
            pieceManager.blackMovable += ret.Count();
            foreach (var move in ret2)
            {
                board.squares[move[0], move[1]].GetComponent<Square>().blackAtkBonus += atkSum * GameManager.instance.bonusRate;
                board.squares[move[0], move[1]].GetComponent<Square>().blackDefBonus += defSum * GameManager.instance.bonusRate;
            }
        }
        return ret;
    }

    internal List<int[]> GetDiagonal(int team, int x, int y)
    {
        List<int[]> ret = new List<int[]>();
        iter = 1;
        while (true)
        {
            if (x - iter < 0 || y - iter < 0)
            {
                break;
            }
            else if (board.squares[x - iter, y - iter].GetComponent<Square>().pieceOnSquare == null)
            {
                ret.Add(new int[] { x - iter, y - iter });
            }
            else if (board.squares[x - iter, y - iter].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().team != team)
            {
                ret.Add(new int[] { x - iter, y - iter });
                break;
            }
            else
            {
                break;
            }
            iter++;
        }
        iter = 1;
        while (true)
        {
            if (x - iter < 0 || y + iter > 7)
            {
                break;
            }
            else if (board.squares[x - iter, y + iter].GetComponent<Square>().pieceOnSquare == null)
            {
                ret.Add(new int[] { x - iter, y + iter });
            }
            else if (board.squares[x - iter, y + iter].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().team != team)
            {
                ret.Add(new int[] { x - iter, y + iter });
                break;
            }
            else
            {
                break;
            }
            iter++;
        }
        iter = 1;
        while (true)
        {
            if (x + iter > 7 || y - iter < 0)
            {
                break;
            }
            else if (board.squares[x + iter, y - iter].GetComponent<Square>().pieceOnSquare == null)
            {
                ret.Add(new int[] { x + iter, y - iter });
            }
            else if (board.squares[x + iter, y - iter].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().team != team)
            {
                ret.Add(new int[] { x + iter, y - iter });
                break;
            }
            else
            {
                break;
            }
            iter++;
        }
        iter = 1;
        while (true)
        {
            if (x + iter > 7 || y + iter > 7)
            {
                break;
            }
            else if (board.squares[x + iter, y + iter].GetComponent<Square>().pieceOnSquare == null)
            {
                ret.Add(new int[] { x + iter, y + iter });
            }
            else if (board.squares[x + iter, y + iter].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().team != team)
            {
                ret.Add(new int[] { x + iter, y + iter });
                break;
            }
            else
            {
                break;
            }
            iter++;
        }

        return ret;
    }

    internal (List<int[]>, List<int[]>) GetDiagonal(int team, int x, int y, float atkSum, float defSum)
    {
        List<int[]> ret = new List<int[]>();
        List<int[]> ret2 = new List<int[]>();
        iter = 1;
        bool flag = true;
        while (flag)
        {
            flag = ValidMove(team, x, -iter, y, -iter, atkSum, defSum, ret, ret2);
            iter++;
        }
        iter = 1;
        flag = true;
        while (flag)
        {
            flag = ValidMove(team, x, -iter, y, iter, atkSum, defSum, ret, ret2);
            iter++;
        }
        iter = 1;
        flag = true;
        while (flag)
        {
            flag = ValidMove(team, x, iter, y, -iter, atkSum, defSum, ret, ret2);
            iter++;
        }
        iter = 1;
        flag = true;
        while (flag)
        {
            flag = ValidMove(team, x, iter, y, iter, atkSum, defSum, ret, ret2);
            iter++;
        }
        return (ret, ret2);
    }

    internal List<int[]> GetKnightMove(int team, int x, int y)
    {
        List<int[]> ret = new List<int[]>();
        if (x - 2 >= 0 && y - 1 >= 0 && (board.squares[x - 2, y - 1].GetComponent<Square>().pieceOnSquare == null || board.squares[x - 2, y - 1].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().team != team))
        {
            ret.Add(new int[] { x - 2, y - 1 });
        }
        if (x - 2 >= 0 && y + 1 < 8 && (board.squares[x - 2, y + 1].GetComponent<Square>().pieceOnSquare == null || board.squares[x - 2, y + 1].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().team != team))
        {
            ret.Add(new int[] { x - 2, y + 1 });
        }
        if (x + 2 < 8 && y - 1 >= 0 && (board.squares[x + 2, y - 1].GetComponent<Square>().pieceOnSquare == null || board.squares[x + 2, y - 1].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().team != team))
        {
            ret.Add(new int[] { x + 2, y - 1 });
        }
        if (x + 2 < 8 && y + 1 < 8 && (board.squares[x + 2, y + 1].GetComponent<Square>().pieceOnSquare == null || board.squares[x + 2, y + 1].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().team != team))
        {
            ret.Add(new int[] { x + 2, y + 1 });
        }
        if (x - 1 >= 0 && y - 2 >= 0 && (board.squares[x - 1, y - 2].GetComponent<Square>().pieceOnSquare == null || board.squares[x - 1, y - 2].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().team != team))
        {
            ret.Add(new int[] { x - 1, y - 2 });
        }
        if (x - 1 >= 0 && y + 2 < 8 && (board.squares[x - 1, y + 2].GetComponent<Square>().pieceOnSquare == null || board.squares[x - 1, y + 2].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().team != team))
        {
            ret.Add(new int[] { x - 1, y + 2 });
        }
        if (x + 1 < 8 && y - 2 >= 0 && (board.squares[x + 1, y - 2].GetComponent<Square>().pieceOnSquare == null || board.squares[x + 1, y - 2].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().team != team))
        {
            ret.Add(new int[] { x + 1, y - 2 });
        }
        if (x + 1 < 8 && y + 2 < 8 && (board.squares[x + 1, y + 2].GetComponent<Square>().pieceOnSquare == null || board.squares[x + 1, y + 2].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().team != team))
        {
            ret.Add(new int[] { x + 1, y + 2 });
        }
        return ret;
    }

    internal (List<int[]>, List<int[]>) GetKnightMove(int team, int x, int y, float atkSum, float defSum)
    {
        List<int[]> ret = new List<int[]>();
        List<int[]> ret2 = new List<int[]>();
        ValidMove(team, x, -2, y, -1, atkSum, defSum, ret, ret2);
        ValidMove(team, x, -2, y, 1, atkSum, defSum, ret, ret2);
        ValidMove(team, x, 2, y, -1, atkSum, defSum, ret, ret2);
        ValidMove(team, x, 2, y, 1, atkSum, defSum, ret, ret2);
        ValidMove(team, x, -1, y, -2, atkSum, defSum, ret, ret2);
        ValidMove(team, x, -1, y, 2, atkSum, defSum, ret, ret2);
        ValidMove(team, x, 1, y, -2, atkSum, defSum, ret, ret2);
        ValidMove(team, x, 1, y, 2, atkSum, defSum, ret, ret2);
        return (ret, ret2);
    }

    internal List<int[]> GetStraight(int team, int x, int y)
    {
        List<int[]> ret = new List<int[]>();
        iter = 1;
        while (true)
        {
            if (x - iter < 0)
            {
                break;
            }
            else if (board.squares[x - iter, y].GetComponent<Square>().pieceOnSquare == null)
            {
                ret.Add(new int[] { x - iter, y });
            }
            else if (board.squares[x - iter, y].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().team != team)
            {
                ret.Add(new int[] { x - iter, y });
                break;
            }
            else
            {
                break;
            }
            iter++;
        }
        iter = 1;
        while (true)
        {
            if (x + iter > 7)
            {
                break;
            }
            else if (board.squares[x + iter, y].GetComponent<Square>().pieceOnSquare == null)
            {
                ret.Add(new int[] { x + iter, y });
            }
            else if (board.squares[x + iter, y].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().team != team)
            {
                ret.Add(new int[] { x + iter, y });
                break;
            }
            else
            {
                break;
            }
            iter++;
        }
        iter = 1;
        while (true)
        {
            if (y - iter < 0)
            {
                break;
            }
            else if (board.squares[x, y - iter].GetComponent<Square>().pieceOnSquare == null)
            {
                ret.Add(new int[] { x, y - iter });
            }
            else if (board.squares[x, y - iter].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().team != team)
            {
                ret.Add(new int[] { x, y - iter });
                break;
            }
            else
            {
                break;
            }
            iter++;
        }
        iter = 1;
        while (true)
        {
            if (y + iter > 7)
            {
                break;
            }
            else if (board.squares[x, y + iter].GetComponent<Square>().pieceOnSquare == null)
            {
                ret.Add(new int[] { x, y + iter });
            }
            else if (board.squares[x, y + iter].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().team != team)
            {
                ret.Add(new int[] { x, y + iter });
                break;
            }
            else
            {
                break;
            }
            iter++;
        }

        return ret;
    }

    internal (List<int[]>, List<int[]>) GetStraight(int team, int x, int y, float atkSum, float defSum)
    {
        List<int[]> ret = new List<int[]>();
        List<int[]> ret2 = new List<int[]>();
        iter = 1;
        bool flag = true;
        while (flag)
        {
            flag = ValidMove(team, x, -iter, y, 0, atkSum, defSum, ret, ret2);
            iter++;
        }
        iter = 1;
        flag = true;
        while (flag)
        {
            flag = ValidMove(team, x, iter, y, 0, atkSum, defSum, ret, ret2);
            iter++;
        }
        iter = 1;
        flag = true;
        while (flag)
        {
            flag = ValidMove(team, x, 0, y, -iter, atkSum, defSum, ret, ret2);
            iter++;
        }
        iter = 1;
        flag = true;
        while (flag)
        {
            flag = ValidMove(team, x, 0, y, iter, atkSum, defSum, ret, ret2);
            iter++;
        }
        return (ret, ret2);
    }

    internal (List<int[]>, List<int[]>) GetPawnMove(int team, int x, int y, float atkSum, float defSum)
    {
        List<int[]> ret = new List<int[]>();
        List<int[]> ret2 = new List<int[]>();
        if(team == 1)//black
        {
            if (x + 1 < 8 && board.squares[x + 1, y].GetComponent<Square>().pieceOnSquare == null)
            {
                ret.Add(new int[] { x + 1, y});
                if (x + 2 < 8 && board.squares[x, y].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().notMoved && board.squares[x + 2, y].GetComponent<Square>().pieceOnSquare == null)
                {
                    ret.Add(new int[] { x + 2, y });
                }
            }
            if (x + 1 < 8 && y - 1 >= 0)
            {
                ret2.Add(new int[] { x + 1, y - 1 });
                if (board.squares[x + 1, y - 1].GetComponent<Square>().pieceOnSquare != null && board.squares[x + 1, y - 1].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().team != team)
                {
                    ret.Add(new int[] { x + 1, y - 1 });
                }
                else if (board.squares[x + 1, y - 1].GetComponent<Square>().pieceOnSquare == null && board.squares[x, y - 1].GetComponent<Square>().pieceOnSquare != null && board.squares[x, y - 1].GetComponent<Square>().pieceOnSquare.GetComponent<Pawn>() != null && board.squares[x, y - 1].GetComponent<Square>().pieceOnSquare.GetComponent<Pawn>().twoSquareAdvance == true)
                {
                    ret.Add(new int[] { x + 1, y - 1 });
                }
            }
            
            if (x + 1 < 8 && y + 1 < 8)
            {
                ret2.Add(new int[] { x + 1, y + 1 });
                if (board.squares[x + 1, y + 1].GetComponent<Square>().pieceOnSquare != null && board.squares[x + 1, y + 1].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().team != team)
                {
                    ret.Add(new int[] { x + 1, y + 1 });
                }
                else if (board.squares[x + 1, y + 1].GetComponent<Square>().pieceOnSquare == null && board.squares[x, y + 1].GetComponent<Square>().pieceOnSquare != null && board.squares[x, y + 1].GetComponent<Square>().pieceOnSquare.GetComponent<Pawn>() != null && board.squares[x, y + 1].GetComponent<Square>().pieceOnSquare.GetComponent<Pawn>().twoSquareAdvance == true)
                {
                    ret.Add(new int[] { x + 1, y + 1 });
                }
            }
        }
        else//white
        {
            if (x - 1 >= 0 && board.squares[x - 1, y].GetComponent<Square>().pieceOnSquare == null)
            {
                ret.Add(new int[] { x - 1, y});
                if (x - 2>= 0 && board.squares[x, y].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().notMoved && board.squares[x - 2, y].GetComponent<Square>().pieceOnSquare == null)
                {
                    ret.Add(new int[] { x - 2, y });
                }
            }
            if (x - 1 >= 0 && y - 1 >= 0)
            {
                ret2.Add(new int[] { x - 1, y - 1 });
                if (board.squares[x - 1, y - 1].GetComponent<Square>().pieceOnSquare != null && board.squares[x - 1, y - 1].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().team != team)
                {
                    ret.Add(new int[] { x - 1, y - 1 });
                }
                else if (board.squares[x - 1, y - 1].GetComponent<Square>().pieceOnSquare == null && board.squares[x, y - 1].GetComponent<Square>().pieceOnSquare != null && board.squares[x, y - 1].GetComponent<Square>().pieceOnSquare.GetComponent<Pawn>() != null && board.squares[x, y - 1].GetComponent<Square>().pieceOnSquare.GetComponent<Pawn>().twoSquareAdvance == true)
                {
                    ret.Add(new int[] { x - 1, y - 1 });
                }
            }
            
            if (x - 1 >= 0 && y + 1 < 8)
            {
                ret2.Add(new int[] { x - 1, y + 1 });
                if (board.squares[x - 1, y + 1].GetComponent<Square>().pieceOnSquare != null && board.squares[x - 1, y + 1].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().team != team)
                {
                    ret.Add(new int[] { x - 1, y + 1 });
                }
                else if (board.squares[x - 1, y + 1].GetComponent<Square>().pieceOnSquare == null && board.squares[x, y + 1].GetComponent<Square>().pieceOnSquare != null && board.squares[x, y + 1].GetComponent<Square>().pieceOnSquare.GetComponent<Pawn>() != null && board.squares[x, y + 1].GetComponent<Square>().pieceOnSquare.GetComponent<Pawn>().twoSquareAdvance == true)
                {
                    ret.Add(new int[] { x - 1, y + 1 });
                }
            }
        }

        return (ret, ret2);
    }

    internal List<int[]> GetPawnAttack(int team, int x, int y)
    {
        List<int[]> ret = new List<int[]>();
        if (team == 1)//black
        {

            if (x + 1 < 8 && y - 1 >= 0 && board.squares[x + 1, y - 1].GetComponent<Square>().pieceOnSquare != null && board.squares[x + 1, y - 1].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().team != team)
            {
                ret.Add(new int[] { x + 1, y - 1 });
            }
            if (x + 1 < 8 && y + 1 < 8 && board.squares[x + 1, y + 1].GetComponent<Square>().pieceOnSquare != null && board.squares[x + 1, y + 1].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().team != team)
            {
                ret.Add(new int[] { x + 1, y + 1 });
            }
        }
        else//white
        {
            if (x - 1 >= 0 && y - 1 >= 0 && board.squares[x - 1, y - 1].GetComponent<Square>().pieceOnSquare != null && board.squares[x - 1, y - 1].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().team != team)
            {
                ret.Add(new int[] { x - 1, y - 1 });
            }
            if (x - 1 >= 0 && y + 1 < 8 && board.squares[x - 1, y + 1].GetComponent<Square>().pieceOnSquare != null && board.squares[x - 1, y + 1].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().team != team)
            {
                ret.Add(new int[] { x - 1, y + 1 });
            }
        }

        return ret;
    }

    internal (List<int[]>, List<int[]>) GetPawnAttack(int team, int x, int y, float atkSum, float defSum)
    {
        List<int[]> ret = new List<int[]>();
        List<int[]> ret2 = new List<int[]>();
        if (team == 1)//black
        {

            ValidMove(team, x, 1, y, -1, atkSum, defSum, ret, ret2);
            ValidMove(team, x, 1, y, 1, atkSum, defSum, ret, ret2);
        }
        else//white
        {
            ValidMove(team, x, -1, y, 1, atkSum, defSum, ret, ret2);
            ValidMove(team, x, -1, y, 1, atkSum, defSum, ret, ret2);
        }

        return (ret, ret2);
    }

    internal List<int[]> GetKingMove(int team, int x, int y)
    {
        List<int[]> ret = new List<int[]>();
        if (team == 1)//black
        {
            for (int i = -1; i < 2; i++)
            {
                for(int j = -1; j < 2; j++)
                {
                    if(i == 0 && j == 0)
                    {
                        continue;
                    }
                    else if(0 <= x + i && x + i < 8 && 0 <= y + j && y + j < 8)
                    {
                        if(board.squares[x + i, y + j].GetComponent<Square>().pieceOnSquare == null || board.squares[x + i, y + j].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().team != team)
                        {
                            ret.Add(new int[] { x + i, y + j });
                        }
                    }
                }
            }
            if(board.squares[x, y].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().notMoved)//Castling, need Check option
            {
                if(board.squares[x, y + 3].GetComponent<Square>().pieceOnSquare != null && board.squares[x, y + 3].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().notMoved)
                {
                    if(board.squares[x, y + 1].GetComponent<Square>().pieceOnSquare == null && board.squares[x, y + 2].GetComponent<Square>().pieceOnSquare == null)
                    {
                        ret.Add(new int[] {x, y + 2});
                    }
                }
                if (board.squares[x, y - 4].GetComponent<Square>().pieceOnSquare != null && board.squares[x, y - 4].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().notMoved)
                {
                    if (board.squares[x, y - 1].GetComponent<Square>().pieceOnSquare == null && board.squares[x, y - 2].GetComponent<Square>().pieceOnSquare == null && board.squares[x, y - 3].GetComponent<Square>().pieceOnSquare == null)
                    {
                        ret.Add(new int[] { x, y - 2 });
                    }
                }
            }
        }
        else//white
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        continue;
                    }
                    else if (0 <= x + i && x + i < 8 && 0 <= y + j && y + j < 8)
                    {
                        if (board.squares[x + i, y + j].GetComponent<Square>().pieceOnSquare == null || board.squares[x + i, y + j].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().team != team)
                        {
                            ret.Add(new int[] { x + i, y + j });
                        }
                    }
                }
            }
            if (board.squares[x, y].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().notMoved)//Castling, need Check option
            {
                if (board.squares[x, y + 3].GetComponent<Square>().pieceOnSquare != null && board.squares[x, y + 3].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().notMoved)
                {
                    if (board.squares[x, y + 1].GetComponent<Square>().pieceOnSquare == null && board.squares[x, y + 2].GetComponent<Square>().pieceOnSquare == null)
                    {
                        ret.Add(new int[] { x, y + 2 });
                    }
                }
                if (board.squares[x, y - 4].GetComponent<Square>().pieceOnSquare != null && board.squares[x, y - 4].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().notMoved)
                {
                    if (board.squares[x, y - 1].GetComponent<Square>().pieceOnSquare == null && board.squares[x, y - 2].GetComponent<Square>().pieceOnSquare == null && board.squares[x, y - 3].GetComponent<Square>().pieceOnSquare == null)
                    {
                        ret.Add(new int[] { x, y - 2 });
                    }
                }
            }
        }
        return ret;
    }

    internal List<int[]> GetKingAttack(int team, int x, int y)
    {
        List<int[]> ret = new List<int[]>();
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                else if (0 <= x + i && x + i < 8 && 0 <= y + j && y + j < 8)
                {
                    if (board.squares[x + i, y + j].GetComponent<Square>().pieceOnSquare == null || board.squares[x + i, y + j].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().team != team)
                    {
                        ret.Add(new int[] { x + i, y + j });
                    }
                }
            }
        }
        return ret;
    }

    bool ValidMove(int team, int x, int xMod, int y, int yMod, float atkSum, float defSum, List<int[]> ret, List<int[]> ret2)
    {
        bool valid = false;
        if (x + xMod >= 0 && x + xMod < 8 && y + yMod >= 0 && y + yMod < 8)
        {
            Square square = board.squares[x + xMod, y + yMod].GetComponent<Square>();
            ret2.Add(new int[] { x + xMod, y + yMod });
            if (square.pieceOnSquare == null)
            {
                valid = true;
                ret.Add(new int[] { x + xMod, y + yMod });
            }
            else if (square.pieceOnSquare.GetComponent<Piece>().team != team)
            {
                ret.Add(new int[] { x + xMod, y + yMod });
            }
        }
        return valid;
    }
}
