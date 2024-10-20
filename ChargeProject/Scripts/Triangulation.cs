using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class DelaunayTriangulation
{
    public static List<Triangle> Delaunay(Triangle superTriangle, List<Vector2> points)
    {
        List<Triangle> triangles = new List<Triangle>();
        triangles.Add(superTriangle);
        foreach (Vector2 point in points)
        {
            AddPoint(triangles, point);
        }
        triangles.RemoveAll(t => t.ContainsVertex(superTriangle.p1) || t.ContainsVertex(superTriangle.p2) || t.ContainsVertex(superTriangle.p3));
        return triangles;
    }

    static void AddPoint(List<Triangle> triangles, Vector2 point)
    {
        //Debug.Log("Add " + point);
        // �� ���� ���ο� �ִ� �ﰢ������ ã���ϴ�.
        List<Triangle> badTriangles = new List<Triangle>();
        foreach (Triangle triangle in triangles)
        {
            if (triangle.CircumcircleContains(point))
            {
                badTriangles.Add(triangle);
            }
        }

        List<Edge> polygon = new List<Edge>();

        // �������� �ʴ� ��� ������ ã���ϴ�.
        foreach (Triangle triangle in badTriangles)
        {
            foreach (Edge edge in triangle.edges)
            {
                bool shared = false;
                foreach (Triangle other in badTriangles)
                {
                    if (other != triangle && other.HasEdge(edge))
                    {
                        shared = true;
                        break;
                    }
                }
                if (!shared)
                {
                    polygon.Add(edge);
                }
            }
        }

        // "bad" �ﰢ������ �����մϴ�.
        triangles.RemoveAll(t => badTriangles.Contains(t));

        // ���ο� �ﰢ������ �߰��մϴ�.
        foreach (Edge edge in polygon)
        {
            triangles.Add(new Triangle(edge.p1, edge.p2, point));
        }
    }
}