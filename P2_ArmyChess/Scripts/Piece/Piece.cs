using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public int pieceType;
    public int team;
    public int id;
    public bool notMoved = true;

    public int x;
    public int y;
    public List<int[]> movable = new List<int[]>();
    public List<int[]> attackable = new List<int[]>();

    public List<Soldier> soldiers = new List<Soldier>();

    public void SetSoldier()
    {
        PieceUpgrade upgrade;
        if (team == 0)
        {
            upgrade = GameManager.instance.pieceUpgrades[pieceType];
        }
        else
        {
            upgrade = GameManager.instance.enemyUpgrades[pieceType];
        }
        for (int i = 0; i < upgrade.Number; i++)
        {
            soldiers.Add(new Soldier(pieceType, upgrade.Hp, upgrade.Attack, upgrade.Defense, upgrade.Magic, upgrade.Speed, upgrade.Skills, upgrade.SkillLevel));
        }
    }

    public void MoveTo(Square target)
    {
        x = target.x;
        y = target.y;
    }

    public void Remove()
    {
        gameObject.SetActive(false);
    }
    public virtual void Clean()
    {
        soldiers.Clear();
        movable.Clear();
        attackable.Clear();
        notMoved = true;
    }
}
