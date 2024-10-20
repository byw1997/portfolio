using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Board : MonoBehaviour
{
    public GameObject whiteSquare;
    public GameObject blackSquare;
    internal GameObject[,] squares;

    List<int[]> movable = new List<int[]>();

    internal GetMovement move;
    internal PiecePlacer piecePlacer;
    internal Highlight highlight;
    internal PieceManager pieceManager;

    public bool pieceSelected = false;
    public Square squareSelected;
    Vector3 norm = new Vector3(0, 0.5f, 0);

    List<List<GameObject>> temp;

    public GameObject promotionUI;
    public int promoteType = -1;
    bool boardExist = false;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        move = GetComponent<GetMovement>();
        piecePlacer = GetComponent<PiecePlacer>();
        pieceManager = GetComponent<PieceManager>();
        highlight = GetComponent<Highlight>();
    }

    // Update is called once per frame
    public void CreateBoard(int size)//Recommend to ensure size == 8. At least make sure size % 2 == 0.
    {
        StopAllCoroutines();
        if(boardExist) return;
        boardExist = true;
        squares = new GameObject[size, size];
        float startX = 0.5f - size / 2;
        float startY = -0.5f;
        float startZ = size / 2 - 0.5f;
        Vector3 currentSquare = new Vector3(startX, startY, startZ);
        bool white = true;
        for (int i = 0; i < size; i++)
        {
            for(int j = 0; j < size; j++)//char j+97 ,size - i
            {
                if (white)
                {
                    squares[i, j] = Instantiate(whiteSquare, currentSquare, Quaternion.identity, gameObject.transform);
                }
                else
                {
                    squares[i, j] = Instantiate(blackSquare, currentSquare, Quaternion.identity, gameObject.transform);
                }
                squares[i, j].name = (char)(j + 97) + (size - i).ToString();
                Square temp = squares[i, j].GetComponent<Square>();
                temp.x = i;
                temp.y = j;
                currentSquare.x += 1;
                white = !white;
            }
            currentSquare.x = startX;
            currentSquare.z -= 1;
            white = !white;
        }
    }
    public void CreatePiece()
    {
        //pieceManager.PrepareList();
        for(int i = 0; i < 8; i++)//pawn
        {
            squares[1, i].GetComponent<Square>().pieceOnSquare = piecePlacer.Place(1, 0, squares[1,i]);
            pieceManager.blackPawn.Add(squares[1, i].GetComponent<Square>().pieceOnSquare);
            squares[1, i].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().id = i;
            squares[6, i].GetComponent<Square>().pieceOnSquare = piecePlacer.Place(0, 0, squares[6,i]);
            pieceManager.whitePawn.Add(squares[6, i].GetComponent<Square>().pieceOnSquare);
            squares[6, i].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().id = i;
        }
        for (int i = 0; i < 2; i++)//knight
        {
            squares[0, 5 * i + 1].GetComponent<Square>().pieceOnSquare = piecePlacer.Place(1, 1, squares[0, 5 * i + 1]);
            pieceManager.blackKnight.Add(squares[0, 5 * i + 1].GetComponent<Square>().pieceOnSquare);
            squares[0, 5 * i + 1].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().id = i;
            squares[7, 5 * i + 1].GetComponent<Square>().pieceOnSquare = piecePlacer.Place(0, 1, squares[7, 5 * i + 1]);
            pieceManager.whiteKnight.Add(squares[7, 5 * i + 1].GetComponent<Square>().pieceOnSquare);
            squares[7, 5 * i + 1].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().id = i;
        }
        for (int i = 0; i < 2; i++)//bishop
        {
            squares[0, 3 * i + 2].GetComponent<Square>().pieceOnSquare = piecePlacer.Place(1, 2, squares[0, 3 * i + 2]);
            pieceManager.blackBishop.Add(squares[0, 3 * i + 2].GetComponent<Square>().pieceOnSquare);
            squares[0, 3 * i + 2].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().id = i;
            squares[7, 3 * i + 2].GetComponent<Square>().pieceOnSquare = piecePlacer.Place(0, 2, squares[7, 3 * i + 2]);
            pieceManager.whiteBishop.Add(squares[7, 3 * i + 2].GetComponent<Square>().pieceOnSquare);
            squares[7, 3 * i + 2].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().id = i;
        }
        for (int i = 0; i < 2;  i++)//rook
        {
            squares[0, 7 * i].GetComponent<Square>().pieceOnSquare = piecePlacer.Place(1, 3, squares[0, 7 * i]);
            pieceManager.blackRook.Add(squares[0, 7 * i].GetComponent<Square>().pieceOnSquare);
            squares[0, 7 * i].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().id = i;
            squares[7, 7 * i].GetComponent<Square>().pieceOnSquare = piecePlacer.Place(0, 3, squares[7, 7 * i]);
            pieceManager.whiteRook.Add(squares[7, 7 * i].GetComponent<Square>().pieceOnSquare);
            squares[7, 7 * i].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().id = i;
        }
        squares[0, 3].GetComponent<Square>().pieceOnSquare = piecePlacer.Place(1, 4, squares[0, 3]);//Queen
        pieceManager.blackQueen.Add(squares[0, 3].GetComponent<Square>().pieceOnSquare);
        squares[0, 3].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().id = 0;
        squares[7, 3].GetComponent<Square>().pieceOnSquare = piecePlacer.Place(0, 4, squares[7, 3]);
        pieceManager.whiteQueen.Add(squares[7, 3].GetComponent<Square>().pieceOnSquare);
        squares[7, 3].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().id = 0;
        squares[0, 4].GetComponent<Square>().pieceOnSquare = piecePlacer.Place(1, 5, squares[0, 4]);//King
        pieceManager.blackKing.Add(squares[0, 4].GetComponent<Square>().pieceOnSquare);
        squares[0, 4].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().id = 0;
        pieceManager.blackKingPos[0] = 0;
        pieceManager.blackKingPos[1] = 4;
        squares[7, 4].GetComponent<Square>().pieceOnSquare = piecePlacer.Place(0, 5, squares[7, 4]);
        pieceManager.whiteKing.Add(squares[7, 4].GetComponent<Square>().pieceOnSquare);
        squares[7, 3].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().id = 0;
        pieceManager.whiteKingPos[0] = 7;
        pieceManager.whiteKingPos[1] = 4;
    }

    public void SetTeamForPiece()
    {
        for(int i = 0; i < 2; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                squares[i, j].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().team = 1;
                squares[i, j].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().x = i;
                squares[i, j].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().y = j;
                squares[i, j].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().SetSoldier();
                squares[7 - i, j].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().team = 0;
                squares[7 - i, j].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().x = 7 - i;
                squares[7 - i, j].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().y = j;
                squares[7 - i, j].GetComponent<Square>().pieceOnSquare.GetComponent<Piece>().SetSoldier();
            }
        }
    }

    public void StartTurn(int team)
    {
        bool check = isCheck(team); //Show Check Statement
        ComputeMove(team, true);
        if(team == 0 && pieceManager.whiteMovable == 0)
        {
            if(check)
            {
                CheckMate(team);
            }
            else
            {
                StaleMate();
            }
            return;
        }
        else if(team == 1 && pieceManager.blackMovable == 0)
        {
            if (check)
            {
                CheckMate(team);
            }
            else
            {
                StaleMate();
            }
            return;
        }
        GameManager.instance.chessTurn = team;
    }

    void CheckMate(int team)
    {
        GameManager.instance.StageEnd(1 - team);
    }

    void StaleMate()
    {
        GameManager.instance.RestartStage();
    }

    internal bool isCheck(int team)//Does enemy checking 'team'?
    {
        if (team == 0)
        {
            temp = pieceManager.blackPiece;
        }
        else
        {
            temp = pieceManager.whitePiece;
        }
        foreach(var pieces in temp)
        {
            foreach(var piece in pieces)
            {
                if (piece)
                {
                    Piece currentPiece = piece.GetComponent<Piece>();
                    if (move.IsChecking(currentPiece.team, currentPiece.x, currentPiece.y, currentPiece.pieceType))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }


    void ComputeMove(int team, bool resetTwoSquareAdvance)
    {
        if (team == 0)
        {
            temp = pieceManager.whitePiece;
        }
        else
        {
            temp = pieceManager.blackPiece;
        }
        foreach (var pieces in temp)
        {
            foreach (var piece in pieces)
            {
                if (piece)
                {
                    Piece currentPiece = piece.GetComponent<Piece>();
                    if(currentPiece.pieceType == 0 && resetTwoSquareAdvance)
                    {
                        piece.GetComponent<Pawn>().twoSquareAdvance = false;
                    }
                    float atkSum = 0f;
                    float defSum = 0f;
                    foreach(Soldier soldier in currentPiece.soldiers)
                    {
                        atkSum += soldier.attack;
                        defSum += soldier.defense;
                    }
                    currentPiece.movable = move.GetMove(team, currentPiece.x, currentPiece.y, currentPiece.pieceType, atkSum, defSum);
                }
            }
        }
    }

    void CleanBonus()
    {
        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                Square square = squares[i, j].GetComponent<Square>();
                square.CleanBonus();
            }
        }
    }
    
    public void HighlightMovable(int x, int y)
    {
        Debug.Log("Highlight");
        Square currentSquare = squares[x,y].GetComponent<Square>();
        
        if(currentSquare.pieceOnSquare != null)
        {
            Piece currentPiece = currentSquare.pieceOnSquare.GetComponent<Piece>();
            Debug.Log(x + ", " + y + "piece soldier has " + currentPiece.soldiers.Count);
            squareSelected = currentSquare;
            pieceSelected = true;
            movable = currentPiece.movable;
        }
        
        foreach(var possibleMove in movable)
        {
            squares[possibleMove[0], possibleMove[1]].GetComponent<Square>().Highlight();
        }

    }

    public void UnHighlightMovable()
    {
        pieceSelected = false;
        squareSelected = null;
        foreach (var possibleMove in movable)
        {
            squares[possibleMove[0], possibleMove[1]].GetComponent<Square>().UnHighlight();
        }
        Debug.Log("Unhighlight");
    }

    public void PieceAction(Square target)//Need report move
    {
        squareSelected.pieceOnSquare.GetComponent<Piece>().notMoved = false;
        Battle(target);
    }

    void Battle(Square target)
    {
        bool win = true;
        if (target.pieceOnSquare == null)
        {
            if (squareSelected.pieceOnSquare.GetComponent<King>() != null && Mathf.Abs(target.y - squareSelected.y) == 2)
            {
                Castling(target, target.y - squareSelected.y);
            }
            else if (squareSelected.pieceOnSquare.GetComponent<Pawn>() != null && Mathf.Abs(target.x - squareSelected.x) == 2)//Two-Square advanced
            {
                squareSelected.pieceOnSquare.GetComponent<Pawn>().twoSquareAdvance = true;
                MovePiece(target);
            }
            else if (squareSelected.pieceOnSquare.GetComponent<Pawn>() != null && Mathf.Abs(target.x - squareSelected.x) == 1 && Mathf.Abs(target.y - squareSelected.y) == 1 )//En passant
            {
                Pawn Enpassant = squares[squareSelected.x, target.y].GetComponent<Square>().pieceOnSquare.GetComponent<Pawn>();
                if( Enpassant != null && Enpassant.twoSquareAdvance == true)
                {
                    //call battle
                    if (win)
                    {
                        EnPassant(target);
                    }
                    else
                    {
                        Debug.Log("Lost");
                    }
                    //
                }
            }
            else
            {
                MovePiece(target);
            }
            StartCoroutine(EndTurn(target));
        }
        else
        {
            StartCoroutine(CallTurnBasedBattle(target, () => TakePiece(target), () => RemovePiece(squareSelected), () => Debug.Log("Couldn't Take Piece"), () => { RemovePiece(target); RemovePiece(squareSelected); }));
        }
    }

    IEnumerator CallTurnBasedBattle(Square target, System.Action onAttackerKilledTarget, System.Action onTargetKilledAttacker, System.Action onBothSurvived, System.Action onBothDead)
    {
        int win = 0;
        yield return StartCoroutine(GameManager.instance.StartCombat(squareSelected, target, (result) =>{win = result;}));
        Debug.Log("Call back from StartCombat, result: " + win);
        switch (win)
        {
            case 0://Attacker survived, Target terminated.
                onAttackerKilledTarget(); break;
            case 1://Attacker terminated, target survived.
                onTargetKilledAttacker(); break;
            case 2://Both survived.
                onBothSurvived(); break;
            case 3://Both terminated.
                onBothDead(); break;
        }
        StartCoroutine(EndTurn(target));
    }

    void RemovePiece(Square target)
    {
        Piece piece = target.pieceOnSquare.GetComponent<Piece>();
        if (piece.team == 0)
        {
            pieceManager.whitePiece[piece.pieceType][piece.id] = null;
        }
        else
        {
            pieceManager.blackPiece[piece.pieceType][piece.id] = null;
        }
        target.pieceOnSquare.SetActive(false);
        target.pieceOnSquare = null;
    }

    void MovePiece(Square target)
    {
        squareSelected.pieceOnSquare.transform.position = squares[target.x, target.y].transform.position + norm;
        squareSelected.MoveTo(target);
    }

    void TakePiece(Square target)
    {
        RemovePiece(target);
        MovePiece(target);
    }

    void EnPassant(Square target)
    {
        Square enPassant = squares[squareSelected.x, target.y].GetComponent<Square>();
        Piece piece = enPassant.pieceOnSquare.GetComponent<Piece>();
        if (piece.team == 0)
        {
            pieceManager.whitePiece[piece.pieceType][piece.id] = null;
        }
        else
        {
            pieceManager.blackPiece[piece.pieceType][piece.id] = null;
        }
        enPassant.pieceOnSquare.SetActive(false);
        enPassant.pieceOnSquare = null;
        MovePiece(target);
    }

    void Castling(Square target, int side)
    {
        squareSelected.pieceOnSquare.transform.position = squares[target.x, target.y].transform.position + norm;
        squareSelected.MoveTo(target);
        Square rookBeforeCastle = side > 0 ? squares[target.x, target.y + side / 2].GetComponent<Square>() : squares[target.x, target.y + side].GetComponent<Square>();
        Square rookAfterCastle = squares[target.x, target.y - side / 2].GetComponent<Square>();
        rookBeforeCastle.pieceOnSquare.transform.position = squares[target.x, target.y - side / 2].transform.position + norm;
        rookBeforeCastle.MoveTo(rookAfterCastle);
    }

    IEnumerator Promote(Pawn pawn)
    {
        int id = pawn.id;
        int team = pawn.team;

        GameManager.instance.chessTurn = team + 2;
        promotionUI.SetActive(true);
        while(promoteType < 0)
        {
            yield return null;
        }


        Square square = squares[pawn.x, pawn.y].GetComponent<Square>();
        square.pieceOnSquare = piecePlacer.Place(team, promoteType, squares[pawn.x, pawn.y]);
        if(team == 0)
        {
            temp = pieceManager.whitePiece;
        }
        else
        {
            temp = pieceManager.blackPiece;
        }
        temp[promoteType].Add(square.pieceOnSquare);
        Piece piece = square.pieceOnSquare.GetComponent<Piece>();
        piece.id = temp[promoteType].Count - 1;
        piece.team = team;
        piece.x = pawn.x;
        piece.y = pawn.y;
        piece.SetSoldier();


        pawn.Remove();
        promotionUI.SetActive(false);
        promoteType = -1;
        GameManager.instance.chessTurn = team;
    }

    public void PromoteTo(int type)
    {
        promoteType = type;
    }

    IEnumerator EndTurn(Square target)
    {
        Pawn pawn = target.pieceOnSquare.GetComponent<Pawn>();
        if (pawn != null && target.x == 7 * GameManager.instance.chessTurn)
        {
            yield return StartCoroutine(Promote(pawn));
        }
        UnHighlightMovable();
        CleanBonus();
        int currentTurn = GameManager.instance.chessTurn;
        GameManager.instance.chessTurn = -1;
        ComputeMove(currentTurn, false);
        pieceManager.CleanMovableNumber();
        int nextTurn = 1 - currentTurn;
        GameManager.instance.playerTeam = 1 - GameManager.instance.playerTeam;//Player plays both side
        StartTurn(nextTurn);
    }

    public void Clean()
    {
        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                squares[i, j].GetComponent<Square>().Clean();
            }
        }
        pieceManager.Clean();
        piecePlacer.Clean();
    }
}
