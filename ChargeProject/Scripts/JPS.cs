using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class JPS
{
    public JPS() { }

    public Vector2Int? FindNextDirection(Vector2Int start, Vector2Int end, Tilemap tilemap)
    {
        List<JPSNode> openList = new List<JPSNode>();
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

        JPSNode startNode = new JPSNode(start, null, 0, Heuristic(start, end));
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            JPSNode currentNode = GetNodeWithLowestF(openList);

            if (currentNode.Position == end)
            {
                return null; // 목적지 도달 시
            }

            openList.Remove(currentNode);
            closedSet.Add(currentNode.Position);

            foreach (Vector2Int neighbor in GetNeighbors(currentNode, tilemap))
            {
                JPSNode jumpNode = Jump(neighbor, currentNode.Position, end, tilemap);
                if (jumpNode != null && !closedSet.Contains(jumpNode.Position))
                {
                    float tentativeG = currentNode.G + Vector2Int.Distance(currentNode.Position, jumpNode.Position);

                    JPSNode existingNode = openList.Find(node => node.Position == jumpNode.Position);
                    if (existingNode == null)
                    {
                        jumpNode.G = tentativeG;
                        jumpNode.Parent = currentNode;
                        openList.Add(jumpNode);
                    }
                    else if (tentativeG < existingNode.G)
                    {
                        existingNode.G = tentativeG;
                        existingNode.Parent = currentNode;
                    }
                }
            }

            // 다음으로 이동할 방향 반환
            if (currentNode.Parent != null)
            {
                return currentNode.Position - currentNode.Parent.Position;
            }
        }

        return null; // 경로가 없을 경우
    }

    private JPSNode GetNodeWithLowestF(List<JPSNode> openList)
    {
        JPSNode lowestFNode = openList[0];
        foreach (JPSNode node in openList)
        {
            if (node.F < lowestFNode.F)
            {
                lowestFNode = node;
            }
        }
        return lowestFNode;
    }

    private float Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private List<Vector2Int> GetNeighbors(JPSNode node, Tilemap tilemap)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 0), new Vector2Int(-1, 0),
            new Vector2Int(0, 1), new Vector2Int(0, -1),
            new Vector2Int(1, 1), new Vector2Int(-1, 1),
            new Vector2Int(1, -1), new Vector2Int(-1, -1)
        };

        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighborPos = node.Position + direction;
            if (IsWalkable(neighborPos, tilemap))
            {
                neighbors.Add(neighborPos);
            }
        }

        return neighbors;
    }

    private bool IsWalkable(Vector2Int position, Tilemap tilemap)
    {
        TileBase tile = tilemap.GetTile((Vector3Int)position);
        return tile == null; // Tile이 없으면 이동 가능 (장애물이 아님)
    }

    private JPSNode Jump(Vector2Int current, Vector2Int previous, Vector2Int end, Tilemap tilemap)
    {
        Vector2Int direction = current - previous;

        while (IsWalkable(current, tilemap))
        {
            if (current == end)
            {
                return new JPSNode(current, null, 0, 0);
            }

            if (HasForcedNeighbors(current, direction, tilemap))
            {
                return new JPSNode(current, null, 0, Heuristic(current, end));
            }

            if (direction.x != 0 && direction.y != 0)
            {
                if (Jump(new Vector2Int(current.x + direction.x, current.y), current, end, tilemap) != null ||
                    Jump(new Vector2Int(current.x, current.y + direction.y), current, end, tilemap) != null)
                {
                    return new JPSNode(current, null, 0, Heuristic(current, end));
                }
            }

            current += direction;
        }

        return null;
    }

    private bool HasForcedNeighbors(Vector2Int current, Vector2Int direction, Tilemap tilemap)
    {
        if (direction.x != 0 && direction.y != 0)
        {
            return (!IsWalkable(current + new Vector2Int(-direction.x, 0), tilemap) && IsWalkable(current + new Vector2Int(-direction.x, direction.y), tilemap)) ||
                   (!IsWalkable(current + new Vector2Int(0, -direction.y), tilemap) && IsWalkable(current + new Vector2Int(direction.x, -direction.y), tilemap));
        }
        else if (direction.x != 0)
        {
            return (!IsWalkable(current + new Vector2Int(0, 1), tilemap) && IsWalkable(current + new Vector2Int(direction.x, 1), tilemap)) ||
                   (!IsWalkable(current + new Vector2Int(0, -1), tilemap) && IsWalkable(current + new Vector2Int(direction.x, -1), tilemap));
        }
        else if (direction.y != 0)
        {
            return (!IsWalkable(current + new Vector2Int(1, 0), tilemap) && IsWalkable(current + new Vector2Int(1, direction.y), tilemap)) ||
                   (!IsWalkable(current + new Vector2Int(-1, 0), tilemap) && IsWalkable(current + new Vector2Int(-1, direction.y), tilemap));
        }

        return false;
    }
}
