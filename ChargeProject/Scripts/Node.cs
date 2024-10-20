using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node
{
    public int i1, i2;
    public float distance;
    public Vector2Int v1, v2;

    public Node(int i1, int i2, (Vector2Int, Vector2Int, float) result)
    {
        this.i1 = i1;
        this.i2 = i2;
        (this.v1, this.v2, this.distance) = result;
    }

    public bool IsSame(Node other)
    {
        return (i1 == other.i1 && i2 == other.i2) || (i1 == other.i2 && i2 == other.i1);
    }
}

public class Edge
{
    public Vector2 p1, p2;
    public float distance;

    public Edge(Vector2 p1, Vector2 p2)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.distance = Vector2.Distance(p1, p2);
    }

    public Edge(Vector2 p1, Vector2 p2, float distance)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.distance = distance;
    }
    
    public bool IsSame(Edge other)
    {
        return (p1 == other.p1 && p2 == other.p2) || (p1 == other.p2 && p2 == other.p1);
    }
}

public class Triangle
{
    public Vector2 p1, p2, p3;
    public Edge[] edges;

    public Triangle(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.p3 = p3;
        edges = new Edge[3];
        edges[0] = new Edge(p1, p2);
        edges[1] = new Edge(p2, p3);
        edges[2] = new Edge(p3, p1);
    }

    public bool ContainsVertex(Vector2 v)
    {
        return p1 == v || p2 == v || p3 == v;
    }

    public bool HasEdge(Edge e)
    {
        return e.IsSame(edges[0]) || e.IsSame(edges[1]) || e.IsSame(edges[2]);
    }

    // 외접원을 통해 삼각형이 포인트를 포함하는지 확인합니다.
    public bool CircumcircleContains(Vector2 point)
    {
        float ax = p1.x - point.x;
        float ay = p1.y - point.y;
        float bx = p2.x - point.x;
        float by = p2.y - point.y;
        float cx = p3.x - point.x;
        float cy = p3.y - point.y;

        float det_ab = ax * by - ay * bx;
        float det_bc = bx * cy - by * cx;
        float det_ca = cx * ay - cy * ax;

        float a = ax * ax + ay * ay;
        float b = bx * bx + by * by;
        float c = cx * cx + cy * cy;

        float det = a * det_bc + b * det_ca + c * det_ab;

        return det > 0;
    }
}

public class EdgeComparer : IComparer<Edge>
{
    public int Compare(Edge e1, Edge e2)
    {
        float len1 = e1.distance;
        float len2 = e2.distance;
        return len1.CompareTo(len2);
    }
}

public class NodeComparer : IComparer<Node>
{
    public int Compare(Node n1, Node n2)
    {
        float len1 = n1.distance;
        float len2 = n2.distance;
        return len1.CompareTo(len2);
    }
}

public class UnionFind
{
    private int[] parent;
    private int[] rank;

    public UnionFind(int size)
    {
        parent = new int[size];
        rank = new int[size];
        for (int i = 0; i < size; i++)
        {
            parent[i] = i;
            rank[i] = 0;
        }
    }

    public int Find(int x)
    {
        if (parent[x] != x)
        {
            parent[x] = Find(parent[x]);
        }
        return parent[x];
    }

    public void Union(int x, int y)
    {
        int rootX = Find(x);
        int rootY = Find(y);

        if (rootX != rootY)
        {
            if (rank[rootX] > rank[rootY])
            {
                parent[rootY] = rootX;
            }
            else if (rank[rootX] < rank[rootY])
            {
                parent[rootX] = rootY;
            }
            else
            {
                parent[rootY] = rootX;
                rank[rootX]++;
            }
        }
    }
}

public class AStarNode
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

public class JPSNode
{
    public Vector2Int Position { get; set; }
    public JPSNode Parent { get; set; }
    public float G { get; set; } // Start Node와의 거리
    public float H { get; set; } // End Node와의 추정 거리
    public float F => G + H; // G + H 값

    public JPSNode(Vector2Int position, JPSNode parent, float g, float h)
    {
        Position = position;
        Parent = parent;
        G = g;
        H = h;
    }
}