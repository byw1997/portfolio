using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarPathfinding : MonoBehaviour
{
    public Tilemap tilemap;

    private Vector3Int[] directions = {
        new Vector3Int(1, 0, 0),   // 오른쪽
        new Vector3Int(-1, 0, 0),  // 왼쪽
        new Vector3Int(0, 1, 0),   // 위쪽
        new Vector3Int(0, -1, 0)   // 아래쪽
    };

    public List<Vector3> FindPath(Vector3 startWorldPos, Vector3 targetWorldPos)
    {
        Vector3Int startPos = tilemap.WorldToCell(startWorldPos);
        Vector3Int targetPos = tilemap.WorldToCell(targetWorldPos);

        // 오픈 리스트와 클로즈 리스트 생성
        List<AStarNode> openList = new List<AStarNode>();
        HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>();

        // 시작 노드 생성 및 오픈 리스트에 추가
        AStarNode startNode = new AStarNode(startPos, null, 0, GetHeuristic(startPos, targetPos));
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            // 현재 노드를 오픈 리스트에서 찾기
            AStarNode currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].F < currentNode.F)
                {
                    currentNode = openList[i];
                }
            }

            // 현재 노드가 목표에 도달했는지 확인
            if (currentNode.Position == targetPos)
            {
                return ConstructPath(currentNode);
            }

            openList.Remove(currentNode);
            closedSet.Add(currentNode.Position);

            // 인접한 노드를 탐색
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

        // 경로를 찾지 못한 경우 빈 리스트 반환
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
        // 휴리스틱 계산 (맨해튼 거리)
        return Mathf.Abs(pos.x - target.x) + Mathf.Abs(pos.y - target.y);
    }

    private class AStarNode
    {
        public Vector3Int Position;
        public AStarNode Parent;
        public float G; // 시작 노드부터 현재 노드까지의 비용
        public float H; // 휴리스틱 (목표 노드까지의 예상 비용)
        public float F => G + H; // 총 비용

        public AStarNode(Vector3Int position, AStarNode parent, float g, float h)
        {
            Position = position;
            Parent = parent;
            G = g;
            H = h;
        }
    }
}
