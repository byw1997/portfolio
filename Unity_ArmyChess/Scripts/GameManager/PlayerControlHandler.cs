using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerControlHandler : MonoBehaviour
{
    public Board board;
    public CombatManager combatManager;
    internal void GetInput(int team, int turn)
    {
        if (turn == team)//Player turn on chess board
        {
            GetChessInput(team);
        }
        else if(turn == 4 || turn == 5)
        {
            GetCombatInput();
        }
    }

    void GetChessInput(int team)
    {
        Camera mainCamera = GameManager.instance.mainCamera;
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            int layerMask = LayerMask.GetMask("Square");

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.collider.CompareTag("Square"))
                {
                    Square square = hit.collider.GetComponent<Square>();
                    Debug.Log(square.x + ", " + square.y);
                    if (board.pieceSelected)
                    {
                        if (square.highlighted)
                        {
                            board.PieceAction(square);
                            return;
                        }
                        else
                        {
                            board.UnHighlightMovable();
                            if (square == board.squareSelected)
                            {
                                return;
                            }
                        }
                    }

                    if (square.pieceOnSquare != null)
                    {
                        Piece piece = square.pieceOnSquare.GetComponent<Piece>();
                        if (piece.team == team)
                        {
                            board.HighlightMovable(square.x, square.y);
                        }
                    }


                }
                else
                {
                    if (board.pieceSelected)
                    {
                        board.UnHighlightMovable();
                    }
                }
            }

        }
    }
    
    void GetCombatInput()
    {
        Camera combatCamera = combatManager.combatCamera;
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = combatCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            int layerMask = LayerMask.GetMask("Soldier");

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.collider.CompareTag("Soldier"))
                {
                    SoldierLocation location = hit.collider.GetComponentInParent<SoldierLocation>();
                    if (combatManager.soldierSelected == -1)//not selected yet. Select sodier as attacker.
                    {
                        combatManager.SelectSoldier(location.locationId);
                    }

                    else//soldier selected. if skill selected, set Command for soldier.
                    {
                        if(combatManager.skillSelected == -1)
                        {
                            combatManager.UnselectSoldier();
                            combatManager.SelectSoldier(location.locationId);
                        }
                        else
                        {
                            int selectedSoldierTeam = combatManager.soldierSelected / 3;
                            int selectedSoldierId = combatManager.soldierSelected % 3;
                            Soldier selectedSoldier;
                            if (selectedSoldierTeam == 0)
                            {
                                selectedSoldier = combatManager.whiteSoldier[selectedSoldierId];
                            }
                            else
                            {
                                selectedSoldier = combatManager.blackSoldier[selectedSoldierId];
                            }
                            int targetTeam = location.locationId / 3;
                            int targetId = location.locationId % 3;
                            if (selectedSoldierTeam == targetTeam && selectedSoldier.skills[combatManager.skillSelected].allyTarget)//If attacker and target is ally and skill can be used to ally
                            {
                                combatManager.ConfirmCommand(selectedSoldierTeam, selectedSoldierId, targetTeam, targetId, combatManager.skillSelected);
                            }
                            else if (selectedSoldierTeam != targetTeam && selectedSoldier.skills[combatManager.skillSelected].enemyTarget)//If attacker and target is enemy and skill can be used to enemy
                            {
                                combatManager.ConfirmCommand(selectedSoldierTeam, selectedSoldierId, targetTeam, targetId, combatManager.skillSelected);
                            }
                            else
                            {
                                combatManager.UnselectSoldier();
                            }
                            
                        }
                    }
                }
                else
                {
                    combatManager.UnselectSoldier();
                }
            }

        }
    }
}
