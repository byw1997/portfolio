using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;
using Debug = UnityEngine.Debug;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] GameObject startRoom;
    [SerializeField] GameObject shopRoom;
    [SerializeField] GameObject bossRoom;
    [SerializeField] List<GameObject> rooms;
    [SerializeField] Transform grid;
    [SerializeField] TileBase corridorTile;
    [SerializeField] RuleTile wallTile;
    [SerializeField] GameObject lockVertical;
    [SerializeField] GameObject lockHorizontal;
    [SerializeField] GameObject stairTile;
    [SerializeField] List<GameObject> testWeaponChangers;

    float highestX = -10000;
    float highestY = -10000;
    float lowestX = 10000;
    float lowestY = 10000;

    float[,] actualDistance;

    public GameObject newCorridor;
    public GameObject corridor;
    Tilemap corridorTilemap;

    public GameObject newWall;
    public GameObject wall;
    Tilemap wallTilemap;
    GameObject[] activeMap;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    static void Shuffle<T>(IList<T> list)
    {
        Random rng = new Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public void SetTileData(List<GameObject> data, TileBase tileBase)
    {
        startRoom = data[0];
        shopRoom = data[data.Count - 1];
        bossRoom = data[data.Count - 2];
        rooms = data.GetRange(1, data.Count - 3);
        corridorTile = tileBase;
    }

    public IEnumerator GenerateMap()
    {
        GameObject gridobj = new GameObject("Grid");
        gridobj.AddComponent<Grid>();
        grid = gridobj.transform;
        yield return new WaitForFixedUpdate();
        yield return null;
        int numberOfBattle = UnityEngine.Random.Range(7, 10);
        activeMap = new GameObject[numberOfBattle + 3];
        
        Rigidbody2D[] activeRigid = new Rigidbody2D[numberOfBattle + 3];

        int prefabOrder = 0;
        Shuffle(rooms);
        Vector3 randomPoint;
        float ellipseWidth = 30f;
        float ellipseHeight = 15f;
        for (int i = 0; i < numberOfBattle + 3; i++)
        {
            randomPoint = GetRandomPointInEllipse(ellipseWidth, ellipseHeight);
            if(i == 0)
            {
                activeMap[i] = Instantiate(startRoom, new Vector3(0,0,0), Quaternion.identity, grid);
                activeRigid[i] = activeMap[i].GetComponent<Rigidbody2D>();
                activeMap[i].SetActive(false);
            }
            else if (i == numberOfBattle + 1)
            {
                activeMap[i] = Instantiate(shopRoom, randomPoint, Quaternion.identity, grid);
                activeRigid[i] = activeMap[i].GetComponent<Rigidbody2D>();
                activeMap[i].SetActive(false);
            }
            else if (i == numberOfBattle + 2)
            {
                int x = UnityEngine.Random.Range(0, 2);
                int y = UnityEngine.Random.Range(0, 2);
                float randomCos = UnityEngine.Random.Range(0f, 1f);
                float randomSin = Mathf.Sqrt(1 - randomCos * randomCos);
                activeMap[i] = Instantiate(bossRoom, new Vector3(ellipseWidth * (x - 0.5f) * 2 * randomCos, ellipseHeight * (y - 0.5f) * 2 * randomSin, 0), Quaternion.identity, grid);
                activeRigid[i] = activeMap[i].GetComponent<Rigidbody2D>();
                activeMap[i].SetActive(false);
            }
            else
            {
                activeMap[i] = Instantiate(rooms[prefabOrder++], randomPoint, Quaternion.identity, grid);
                activeRigid[i] = activeMap[i].GetComponent<Rigidbody2D>();
                activeMap[i].SetActive(false);
            }
        }
        wall = Instantiate(newWall, Vector3.zero, Quaternion.identity, grid);
        corridor = Instantiate(newCorridor, Vector3.zero, Quaternion.identity, grid);
        foreach (GameObject tilemap in activeMap)
        {
            tilemap.SetActive(true);
        }
        yield return StartCoroutine(CreateCorridor(activeMap, activeRigid));
        foreach(GameObject item in testWeaponChangers)
        {
            item.SetActive(true);
        }
    }

    Vector3 GetRandomPointInEllipse(float width, float height)
    {
        float t = 2 * Mathf.PI * UnityEngine.Random.value;
        float u = UnityEngine.Random.value + UnityEngine.Random.value;
        float r = (u > 1) ? 2 - u : u;
        return new Vector3(width * r * Mathf.Cos(t) / 2, height * r * Mathf.Sin(t) / 2,0);
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    List<Edge> edges;
    List<Triangle> triangles;
    List<Edge> mstEdges;
    List<Node> nodes;
    List<Node> mstNodes;
    List<Vector2> pointVector2;
    List<Vector3Int> pointVector3Int;
    List<BoundsInt> tilemapBounds;
    List<Tilemap> tilemapLists;

    public Corridor cor;
    List<Vector3Int> corridorPoints;

    Triangle testSuperTriangle;

    IEnumerator CreateCorridor(GameObject[] tilemaps, Rigidbody2D[] rigids)
    {
        yield return StartCoroutine(WaitForTileMoving(tilemaps, rigids));
        RoundPosition(tilemaps, rigids);
        Debug.Log("Round Position over");

        List<float[]> points = GetTilemapPosition(tilemaps);

        Triangle superTriangle = GetSuperTriangle();
        testSuperTriangle = superTriangle;
        Debug.Log("Supertriangle = " + superTriangle.p1 + " " +  superTriangle.p2 + " " + superTriangle.p3);

        pointVector2 = new List<Vector2>();
        foreach (float[] point in points)
        {
            Vector2 p = new Vector2(point[0], point[1]);
            pointVector2.Add(p);
        }
        pointVector3Int = new List<Vector3Int>();
        for (int i = 0; i < pointVector2.Count; i++)
        {
            pointVector3Int.Add(new Vector3Int((int)pointVector2[i].x, (int)pointVector2[i].y, 0));
        }
        triangles = DelaunayTriangulation.Delaunay(superTriangle, pointVector2);
        Debug.Log("Triangulation over");

        //Get Actual Distance
        edges = new List<Edge>();
        HashSet<Edge> edgeSet = new HashSet<Edge>();
        nodes = new List<Node>();
        tilemapLists = new List<Tilemap>();
        tilemapBounds = new List<BoundsInt>();
        for(int i = 0; i < tilemaps.Length; i++)
        {
            Tilemap t = tilemaps[i].GetComponent<Tilemap>();
            BoundsInt bound = t.cellBounds;
            tilemapLists.Add(t);
            tilemapBounds.Add(bound);
        }
        // Extract unique edges from triangles and convert to nodes
        foreach (var triangle in triangles)
        {
            foreach (var edge in triangle.edges)
            {
                int index1 = pointVector2.IndexOf(edge.p1);
                int index2 = pointVector2.IndexOf(edge.p2);
                if (!edgeSet.Contains(edge))
                {
                    edgeSet.Add(edge);
                    edges.Add(edge);
                    Node node = new Node(index1, index2, GetDistance(index1, index2));
                    nodes.Add(node);
                }
            }
        }

        FindMST();
        
        //corridor.isStatic = true;
        //corridor.transform.parent = grid.transform;
        corridorTilemap = corridor.GetComponent<Tilemap>();
        TilemapRenderer corridorRenderer = corridor.GetComponent<TilemapRenderer>();
        corridorPoints = new List<Vector3Int>();
        foreach(Node node in mstNodes)
        {
            SetCorridorTile(node);
        }

        //wall = new GameObject("Wall");
        wall.transform.parent = grid.transform;
        wallTilemap = wall.GetComponent<Tilemap>();
        TilemapRenderer wallRenderer = wall.GetComponent<TilemapRenderer>();

        FillWall();

        for(int i = 0; i < activeMap.Length; i++)
        {
            GameObject obj = activeMap[i];
            CompositeCollider2D composite = obj.GetComponent<CompositeCollider2D>();
            if (composite != null)
            {
                DestroyImmediate(composite);
            }
            Rigidbody2D rigid = obj.GetComponent<Rigidbody2D>();
            if (rigid != null)
            {
                DestroyImmediate(rigid);
            }
            TilemapCollider2D tilemapCollider = obj.GetComponent<TilemapCollider2D>();
            tilemapCollider.enabled = true;
            DungeonRoom objRoom;
            if(i == 0)
            {
                objRoom = obj.AddComponent<StartRoom>();
            }
            else if(i == activeMap.Length - 2)
            {
                BossRoom objBossRoom = obj.AddComponent<BossRoom>();
                objBossRoom.stairTile = stairTile;
                GameManager.instance.spawner.stageClear.AddListener(objBossRoom.CreateStair);
                objRoom = objBossRoom;
            }
            else if( i == activeMap.Length - 1)
            {
                objRoom = obj.AddComponent<ShopRoom>();
            }
            else
            {
                objRoom = obj.AddComponent<DungeonRoom>();
            }
            objRoom.t = obj.GetComponent<Tilemap>();
            objRoom.roomActivate.AddListener(GameManager.instance.spawner.CallSpawn);
        }

        //TilemapCollider2D corridorCollider = corridor.GetComponent<TilemapCollider2D>();
        //corridorCollider.isTrigger = true;
        //corridorCollider.usedByComposite= true;
        cor = corridor.GetComponent<Corridor>();
        //cor.lockHorizontal = lockHorizontal;
        //cor.lockVertical = lockVertical;
        cor.halfLockWidth = lockHorizontal.transform.localScale.y / 2;
        GameManager.instance.spawner.waveEnd.AddListener(cor.Unlock);
        cor.corridorPoints = corridorPoints;
        TilemapCollider2D wallCollider = wall.GetComponent<TilemapCollider2D>();
        //Rigidbody2D objrigid = gameObject.GetComponent<Rigidbody2D>();
        //objrigid.bodyType = RigidbodyType2D.Static;
        //CompositeCollider2D objCollider = gameObject.AddComponent<CompositeCollider2D>();
        int walkableLayer = LayerMask.NameToLayer("Walkable");

        if(corridor != null && walkableLayer != -1)
        {
            corridor.layer = walkableLayer;
        }

        int wallLayer = LayerMask.NameToLayer("Wall");

        if (wall != null && wallLayer != -1)
        {
            wall.tag = "Wall";
            wall.layer = wallLayer;
        }

        foreach(GameObject room in activeMap)
        {
            //room.isStatic = true;
        }
    }

    void SetCorridorTile(Node node)
    {
        List<Vector2Int> tilePoints = new List<Vector2Int>();

        int x1 = node.v1.x;
        int y1 = node.v1.y;
        int x2 = node.v2.x;
        int y2 = node.v2.y;

        int dx = Mathf.Abs(x2 - x1);
        int dy = Mathf.Abs(y2 - y1);

        if(dx > dy)
        {
            if(x1 > x2)
            {
                x1++;
                x2--;
            }
            else if(x1 < x2)
            {
                x1--;
                x2++;
            }
            int midY = (y2 + y1) / 2;
            while(y1 !=  midY)
            {
                tilePoints.Add(new Vector2Int(x1, y1));
                if(y1 > midY)
                {
                    y1--;
                }
                else if (y1 < midY)
                {
                    y1++;
                }
            }
            while (y2 != midY)
            {
                tilePoints.Add(new Vector2Int(x2, y2));
                if (y2 > midY)
                {
                    y2--;
                }
                else if (y2 < midY)
                {
                    y2++;
                }
            }
            while (x1 != x2)
            {
                tilePoints.Add(new Vector2Int(x1, midY));
                if(x1 < x2)
                {
                    x1++;
                }
                else if(x1 > x2)
                {
                    x1--;
                }
            }
            tilePoints.Add(new Vector2Int(x2, midY));
        }

        else if (dx < dy)
        {
            if (y1 > y2)
            {
                y1++;
                y2--;
            }
            else if (y1 < y2)
            {
                y1--;
                y2++;
            }
            int midX = (x2 + x1) / 2;
            while (x1 != midX)
            {
                tilePoints.Add(new Vector2Int(x1, y1));
                if (x1 > midX)
                {
                    x1--;
                }
                else if (x1 < midX)
                {
                    x1++;
                }
            }
            while (x2 != midX)
            {
                tilePoints.Add(new Vector2Int(x2, y2));
                if (x2 > midX)
                {
                    x2--;
                }
                else if (x2 < midX)
                {
                    x2++;
                }
            }
            while (y1 != y2)
            {
                tilePoints.Add(new Vector2Int(midX, y1));
                if (y1 < y2)
                {
                    y1++;
                }
                else if (y1 > y2)
                {
                    y1--;
                }
            }
            tilePoints.Add(new Vector2Int(midX, y2));
        }
        List<Vector3Int> ret = new List<Vector3Int>();
        foreach (Vector2Int tilePoint in tilePoints)
        {
            Vector3Int target = new Vector3Int(tilePoint.x, tilePoint.y, 0);
            corridorTilemap.SetTile(target, corridorTile);
            corridorPoints.Add(target);
        }
    }

    void FillWall()
    {
        int maxX = int.MinValue;
        int maxY = int.MinValue;
        int minX = int.MaxValue;
        int minY = int.MaxValue;

        for(int i = 0; i < pointVector2.Count; i++)
        {
            int x1 = (int)pointVector2[i].x + tilemapBounds[i].xMin;
            int y1 = (int)pointVector2[i].y + tilemapBounds[i].yMin;
            int x2 = (int)pointVector2[i].x + tilemapBounds[i].xMax;
            int y2 = (int)pointVector2[i].y + tilemapBounds[i].yMax;
            if (x1 < minX)
            {
                minX = x1;
            }
            if (y1 < minY)
            {
                minY = y1;
            }
            if (x2 > maxX)
            {
                maxX = x2;
            }
            if (y2 > maxY)
            {
                maxY = y2;
            }
        }
        Debug.Log("In FillWall(), max X = " + maxX + " max Y = " + maxY + " min X = " + minX + " min Y = " + minY);
        Vector3Int temp = new Vector3Int(0, 0, 0);
        bool flag = false;
        for(int i = minX-15; i < maxX+15; i++)
        {
            temp.x = i;
            for(int j = minY-15; j < maxY+15; j++)
            {
                temp.y = j;
                flag = false;
                if (corridorTilemap.GetTile(temp) != null)
                {
                    continue;
                }
                for(int k = 0; k < tilemapLists.Count; k++)
                {
                    if (tilemapLists[k].GetTile(temp - pointVector3Int[k]) != null)
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    continue;
                }
                else
                {
                    wallTilemap.SetTile(temp, wallTile);
                }
            }
        }
    }

    (Vector2Int, Vector2Int, float) GetDistance(int i1, int i2)
    {
        int sourceMinX = (int)pointVector2[i1].x + tilemapBounds[i1].xMin;
        int targetMinX = (int)pointVector2[i2].x + tilemapBounds[i2].xMin;
        int sourceMaxX = (int)pointVector2[i1].x + tilemapBounds[i1].xMax - 1;
        int targetMaxX = (int)pointVector2[i2].x + tilemapBounds[i2].xMax - 1;
        int sourceMinY = (int)pointVector2[i1].y + tilemapBounds[i1].yMin;
        int targetMinY = (int)pointVector2[i2].y + tilemapBounds[i2].yMin;
        int sourceMaxY = (int)pointVector2[i1].y + tilemapBounds[i1].yMax - 1;
        int targetMaxY = (int)pointVector2[i2].y + tilemapBounds[i2].yMax - 1;
        int[] sourceBoundX = new int[2];
        int[] targetBoundX = new int[2];
        int[] sourceBoundY = new int[2];
        int[] targetBoundY = new int[2];

        Vector2Int objLocation1 = new Vector2Int((int)pointVector2[i1].x, (int)pointVector2[i1].y);
        Vector2Int objLocation2 = new Vector2Int((int)pointVector2[i2].x, (int)pointVector2[i2].y);

        SetBound(sourceMinX, sourceMaxX, targetMinX, targetMaxX, sourceBoundX, targetBoundX);
        SetBound(sourceMinY, sourceMaxY, targetMinY, targetMaxY, sourceBoundY, targetBoundY);
        int shortestDist = int.MaxValue;
        int dist;
        Vector2Int shortestSource = new Vector2Int(0, 0), shortestTarget = new Vector2Int(0, 0), source = new Vector2Int(0, 0), target = new Vector2Int(0, 0);
        //return (source, target, 0f);
        //Debug.Log(tilemapLists[i1].name + " to " + tilemapLists[i2].name +"\nSource Bound = (" + sourceBoundX[0] + " - " + sourceBoundX[1] + ", " + sourceBoundY[0] + " - " + sourceBoundY[1] + ")\n" + "Target Bound = (" + targetBoundX[0] + " - " + targetBoundX[1] + ", " + targetBoundY[0] + " - " + targetBoundY[1] + ")");
        for(int i = sourceBoundX[0]; i <= sourceBoundX[1]; i++)
        {
            source.x = i;
            for (int j = sourceBoundY[0]; j <= sourceBoundY[1]; j++)
            {
                source.y = j;
                if (tilemapLists[i1].GetTile((Vector3Int)(source - objLocation1)) != null) 
                {
                    for (int ii = targetBoundX[0]; ii <= targetBoundX[1]; ii++)
                    {
                        target.x = ii;
                        for (int jj = targetBoundY[0]; jj <= targetBoundY[1]; jj++)
                        {
                            target.y = jj;
                            if (tilemapLists[i2].GetTile((Vector3Int)(target - objLocation2)) != null)
                            {
                                dist = Mathf.Abs(target.x - source.x) + Mathf.Abs(target.y - source.y);
                                if (dist < shortestDist)
                                {
                                    shortestDist = dist;
                                    shortestSource.x = source.x;
                                    shortestSource.y = source.y;
                                    shortestTarget.x = target.x;
                                    shortestTarget.y = target.y;
                                }
                            }
                        }
                    }
                }
            }
        }
        //Debug.Log("Shortest path of " + tilemapLists[i1].name + " to " + tilemapLists[i2].name + " is from " + shortestSource + " to " + shortestTarget);
        if(shortestSource.x < shortestTarget.x)
        {
            shortestSource.x += 1;
            shortestTarget.x -= 1;
        }
        if(shortestSource.x > shortestTarget.x)
        {
            shortestSource.x -= 1;
            shortestTarget.x += 1;
        }
        if (shortestSource.y < shortestTarget.y)
        {
            shortestSource.y += 1;
            shortestTarget.y -= 1;
        }
        if (shortestSource.y > shortestTarget.y)
        {
            shortestSource.y -= 1;
            shortestTarget.y += 1;
        }
        //Debug.Log("Modified path of " + tilemapLists[i1].name + " to " + tilemapLists[i2].name + " is from " + shortestSource + " to " + shortestTarget);
        return (shortestSource, shortestTarget, shortestDist-1);
    }

    void SetBound(int sMin, int sMax, int tMin, int tMax, int[] sBound, int[] tBound)
    {
        if (tMax <= sMin)
        {
            tBound[0] = tMax;
            tBound[1] = tMax;
            sBound[0] = sMin;
            sBound[1] = sMin;
        }
        else if(tMin <= sMin && sMin <= tMax && tMax <= sMax)
        {
            tBound[0] = sMin;
            tBound[1] = tMax;
            sBound[0] = sMin;
            sBound[1] = tMax;
        }
        else if(tMin <= sMin && sMax <= tMax)
        {
            tBound[0] = sMin;
            tBound[1] = sMax;
            sBound[0] = sMin;
            sBound[1] = sMax;
        }
        else if(sMin <= tMin && tMax <= sMax)
        {
            tBound[0] = tMin;
            tBound[1] = tMax;
            sBound[0] = tMin;
            sBound[1] = tMax;
        }
        else if(sMin <= tMin && tMin <= sMax && sMax <= tMax)
        {
            tBound[0] = tMin;
            tBound[1] = sMax;
            sBound[0] = tMin;
            sBound[1] = sMax;
        }
        else if(sMax <= tMin)
        {
            tBound[0] = tMin;
            tBound[1] = tMin;
            sBound[0] = sMax;
            sBound[1] = sMax;
        }
        else
        {
            Debug.Log("Set Bound Error " + " sMin = " + sMin + " sMax = " + sMax + " tMin = " + tMin + " tMax = " + tMax);

        }
    }

    

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if(testSuperTriangle !=  null)
        {
            Gizmos.DrawLine(testSuperTriangle.p1, testSuperTriangle.p2);
            Gizmos.DrawLine(testSuperTriangle.p2, testSuperTriangle.p3);
            Gizmos.DrawLine(testSuperTriangle.p3, testSuperTriangle.p1);
        }
        
        Gizmos.color = Color.black;
        if (triangles != null)
        {
            foreach (Triangle triangle in triangles)
            {
                Gizmos.DrawLine(triangle.p1, triangle.p2);
                Gizmos.DrawLine(triangle.p2, triangle.p3);
                Gizmos.DrawLine(triangle.p3, triangle.p1);
            }
        }
        Gizmos.color = Color.blue;
        if(mstNodes != null)
        {
            foreach(Node node in mstNodes)
            {
                Gizmos.DrawLine(pointVector2[node.i1], pointVector2[node.i2]);
            }
        }
        
        Gizmos.color = Color.red;
        if (mstNodes != null)
        {
            foreach (Node node in mstNodes)
            {
                Gizmos.DrawLine(new Vector3(node.v1.x, node.v1.y,0), new Vector3(node.v2.x, node.v2.y, 0));
            }
        }
    }
    IEnumerator WaitForTileMoving(GameObject[] tilemaps, Rigidbody2D[] rigids)
    {
        bool tilemoving = true;
        Vector3[] lastPosition = new Vector3[tilemaps.Length];
        for (int i = 0; i < tilemaps.Length; i++)
        {
            lastPosition[i] = (Vector3)rigids[i].position;
        }
        yield return null;
        while (tilemoving)
        {
            tilemoving = false;
            for (int i = 0; i < tilemaps.Length; i++)
            {
                Vector3 delta = (Vector3)rigids[i].position - lastPosition[i];
                if (delta.magnitude > 0.001f)
                {
                    tilemoving = true;
                }
                lastPosition[i] = new Vector3(rigids[i].position.x, rigids[i].position.y, 0);
            }
            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("Tile moving over");
    }

    void RoundPosition(GameObject[] tilemaps, Rigidbody2D[] rigids)
    {
        Collider2D currentCollider;
        foreach (GameObject tilemap in tilemaps)
        {
            currentCollider = tilemap.GetComponentInChildren<PolygonCollider2D>();
            currentCollider.enabled = false;

            // position을 정수로 반올림
            Vector3 roundedPosition = new Vector3(
                Mathf.Round(tilemap.transform.position.x),
                Mathf.Round(tilemap.transform.position.y),
                0
            );
            if(roundedPosition.x > highestX)
            {
                highestX = roundedPosition.x;
            }
            if(roundedPosition.y > highestY) { 
                highestY = roundedPosition.y;
            }
            if(roundedPosition.x < lowestX)
            {
                lowestX = roundedPosition.x;
            }
            if(roundedPosition.y < lowestY) { 
                lowestY = roundedPosition.y;
            }
            // 반올림된 position을 설정
            tilemap.transform.position = roundedPosition;
        }
    }

    List<float[]> GetTilemapPosition(GameObject[] tilemaps)
    {
        List<float[]> ret = new List<float[]>();
        for (int i = 0; i < tilemaps.Length; i++)
        {
            float[] temp = new float[2];
            temp[0] = tilemaps[i].transform.position.x;
            temp[1] = tilemaps[i].transform.position.y;
            ret.Add(temp);
        }
        return ret;
    }
    
    Triangle GetSuperTriangle()
    {
        float midX = (highestX + lowestX) / 2;
        float midY = (highestY + lowestY) / 2;
        float distX = highestX - lowestX;
        float distY = highestY - lowestY;
        float superV1 = midX - distX - 1;
        float superV2 = midX + distX + 1;
        Vector2 p1 = new Vector2(superV1, lowestY - 100);
        Vector2 p2 = new Vector2(superV2, lowestY - 100);
        Vector2 p3 = new Vector2(midX, highestY + distY + 100);
        Triangle ret = new Triangle(p1, p2, p3);
        return ret;
    }

    void FindMST()
    {
        
        

        // Sort edges by length
        nodes.Sort(new NodeComparer());

        // Kruskal's algorithm to find MST
        
        mstNodes = new List<Node>();
        UnionFind uf = new UnionFind(nodes.Count);

        foreach (var node in nodes)
        {
            int index1 = node.i1;
            int index2 = node.i2;

            if (uf.Find(index1) != uf.Find(index2))
            {
                uf.Union(index1, index2);
                mstNodes.Add(node);
            }
        }

        int additionalNodes = 0;
        for(int i = 0; i < nodes.Count && additionalNodes < 2; i++)
        {
            bool notContain = true;
            foreach(var node in mstNodes)
            {
                if (node.IsSame(nodes[i]))
                {
                    notContain = false;
                    break;
                }
            }
            if (notContain)
            {
                mstNodes.Add(nodes[i]);
                additionalNodes++;
            }
        }
        /*
        edges.Sort(new EdgeComparer());
        mstEdges = new List<Edge>();

        UnionFind uf2 = new UnionFind(edges.Count);

        foreach (var edge in edges)
        {
            int index1 = pointVector2.IndexOf(edge.p1);
            int index2 = pointVector2.IndexOf(edge.p2);

            if (uf2.Find(index1) != uf2.Find(index2))
            {
                uf2.Union(index1, index2);
                mstEdges.Add(edge);
            }
        }
        */
        Debug.Log("MST construction over");
    }

}
