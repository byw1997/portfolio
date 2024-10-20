using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarPathfinding : MonoBehaviour
{
    public Tilemap tilemap;

    private Vector3Int[] directions = {
        new Vector3Int(1, 0, 0),   // ������
        new Vector3Int(-1, 0, 0),  // ����
        new Vector3Int(0, 1, 0),   // ����
        new Vector3Int(0, -1, 0)   // �Ʒ���
    };

    public List<Vector3> FindPath(Vector3 startWorldPos, Vector3 targetWorldPos)
    {
        Vector3Int startPos = tilemap.WorldToCell(startWorldPos);
        Vector3Int targetPos = tilemap.WorldToCell(targetWorldPos);

        // ���� ����Ʈ�� Ŭ���� ����Ʈ ����
        List<AStarNode> openList = new List<AStarNode>();
        HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>();

        // ���� ��� ���� �� ���� ����Ʈ�� �߰�
        AStarNode startNode = new AStarNode(startPos, null, 0, GetHeuristic(startPos, targetPos));
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            // ���� ��带 ���� ����Ʈ���� ã��
            AStarNode currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].F < currentNode.F)
                {
                    currentNode = openList[i];
                }
            }

            // ���� ��尡 ��ǥ�� �����ߴ��� Ȯ��
            if (currentNode.Position == targetPos)
            {
                return ConstructPath(currentNode);
            }

            openList.Remove(currentNode);
            closedSet.Add(currentNode.Position);

            // ������ ��带 Ž��
            foreach (Vector3Int direction in directions)
            {
                Vector3Int neighborPos = currentNode.Position + direction;

                if (closedSet.Contains(neighborPos) || !tilemap.HasTile(neighborPos))
                {
                    continue;
                }

                float tentativeGCost = currentNode.G + 1;
                AStarNode neighborNode = openList.Find(n => n.Position == neighborPos);

                if (neighborNode == null)
                {
                    neighborNode = new AStarNode(neighborPos, currentNode, tentativeGCost, GetHeuristic(neighborPos, targetPos));
                    openList.Add(neighborNode);
                }
                else if (tentativeGCost < neighborNode.G)
                {
                    neighborNode.Parent = currentNode;
                    neighborNode.G = tentativeGCost;
                }
            }
        }

        // ��θ� ã�� ���� ��� �� ����Ʈ ��ȯ
        return new List<Vector3>();
    }

    private List<Vector3> ConstructPath(AStarNode node)
    {
        List<Vector3> path = new List<Vector3>();

        while (node != null)
        {
            path.Add(tilemap.CellToWorld(node.Position));
            node = node.Parent;
        }

        path.Reverse();
        return path;
    }

    private float GetHeuristic(Vector3Int pos, Vector3Int target)
    {
        // �޸���ƽ ��� (����ư �Ÿ�)
        return Mathf.Abs(pos.x - target.x) + Mathf.Abs(pos.y - target.y);
    }

    private class AStarNode
    {
        public Vector3Int Position;
        public AStarNode Parent;
        public float G; // ���� ������ ���� �������� ���
        public float H; // �޸���ƽ (��ǥ �������� ���� ���)
        public float F => G + H; // �� ���

        public AStarNode(Vector3Int position, AStarNode parent, float g, float h)
        {
            Position = position;
            Parent = parent;
            G = g;
            H = h;
        }
    }
}
