using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


#if UNITY_EDITOR

[CustomEditor(typeof(HexGrid))]
public class HexGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        HexGrid script = (HexGrid)target;
        if (GUILayout.Button("Generate Grid"))
        {
            script.GenerateGrid();
        }
    }
}

#endif


public class HexGrid : MonoBehaviour
{
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;

    private float scale = 1f;

    private Hex[,] hexes;

    private Pathfinder pathfinder = null;
    //private Hex selectedHex = null;

    [SerializeField] private Hex hexPrefab = null;

    Transform tiles = null; // parent of all hex objects


    private void Awake()
    {
        tiles = transform.Find("Tiles");
        hexes = new Hex[width, height];

        for (int i = 0; i < height; i++)
        {
            Transform row = tiles.GetChild(i);
            for (int j = 0; j < width; j++)
            {
                hexes[j, i] = row.GetChild(j).GetComponent<Hex>();
            }
        }
        pathfinder = new Pathfinder(width, height, hexes);
    }


    private void Start()
    {
        ClearAllHexColors();
    }


    public void GenerateGrid()
    {
        tiles = transform.Find("Tiles");

        while (tiles.childCount > 0)
        {
            DestroyImmediate(tiles.GetChild(0).gameObject);
        }

        for (int i = 0; i < height; i++)
        {
            GameObject row = new GameObject("Row " + i);
            row.transform.parent = tiles;
            row.transform.localPosition = new Vector3(i / 2f, 0f, i * Mathf.Sqrt(3f) / 2f);

            for (int j = 0; j < width; j++)
            {
                Hex hex = Instantiate(hexPrefab, row.transform);
                hex.transform.localPosition = new Vector3(j, 0f, 0f);
                hex.name = "Hex (" + j + "," + i + ")";
                hex.x = j;
                hex.z = i;
            }
        }
    }


    public List<Vector3> OnLeftClick(Vector3 clickPos, Vector3 playerPos)
    {
        Debug.Log("clicked pos: (" + clickPos.x + ", " + clickPos.z + ")");
        Hex clickedHex = GetHex(clickPos);
        Hex playerHex = GetHex(playerPos);

        foreach (Hex hex in hexes) hex.SetColor(Color.white);
        var path = pathfinder.FindPath(playerHex.x, playerHex.z, clickedHex.x, clickedHex.z);

        List<Vector3> result = new List<Vector3>();

        if (path != null)
        {
            for (int i = 0; i < path.Count; i++)
            {
                //Debug.Log(path[i].x + ", " + path[i].z);
                path[i].SetColor((i == 0 || i == path.Count - 1) ? Color.green : Color.cyan);
                result.Add(CellWorldPos(path[i].x, path[i].z));
            }
        }
        else
        {
            Debug.Log("No path found");
            result = null;
        }

        return result;
    }


    public List<Hex> FindPath(Hex start, Hex end)
    {
        var path = pathfinder.FindPath(start.x, start.z, end.x, end.z);

        if (path != null)
        {
            return path;
        }
        else
        {
            //Debug.Log("No path found");
            return null;
        }
    }


    public List<Hex> FindApproach(Hex start, Hex end, int maxDistance)
    {
        for (int distance = maxDistance; distance >= 0; distance--)
        {
            var hexesAtThisDistance = new List<Hex>();

            for (int i = end.x - distance; i <= end.x + distance; i++)
            {
                if (i < 0 || i >= hexes.GetLength(0)) continue;
                for (int j = end.z - distance; j <= end.z + distance; j++)
                {
                    if (j < 0 || j >= hexes.GetLength(1)) continue;
                    if (Distance(hexes[i, j], end) == distance)
                    {
                        hexesAtThisDistance.Add(hexes[i, j]);
                    }
                }
            }

            List<List<Hex>> shortestApproaches = new List<List<Hex>>();
            int shortestApproachCount = 0;
            bool approachFound = false;

            foreach (Hex hex in hexesAtThisDistance)
            {
                var approach = FindPath(start, hex);

                if (approach == null)
                {
                    continue;
                }
                else if (!approachFound)
                {
                    approachFound = true;
                    shortestApproaches.Add(approach);
                    shortestApproachCount = approach.Count;
                }
                else if (approach.Count < shortestApproachCount)
                {
                    shortestApproaches = new List<List<Hex>>() { approach };
                    shortestApproachCount = approach.Count;
                }
                else if (approach.Count == shortestApproachCount)
                {
                    shortestApproaches.Add(approach);
                }
            }

            if (approachFound)
            {
                return shortestApproaches[UnityEngine.Random.Range(0, shortestApproaches.Count - 1)];
            }
        }

        Debug.Log("no approach found");
        return null;
    }


    public List<Hex> FindFlight(Hex start, Hex end, int maxDistance, int maxMove)
    {
        int realMax = Math.Min(maxDistance, Distance(start, end) + maxMove);

        for (int distance = realMax; distance >= 0; distance--)
        {
            var hexesAtThisDistance = new List<Hex>();

            for (int i = end.x - distance; i <= end.x + distance; i++)
            {
                if (i < 0 || i >= hexes.GetLength(0)) continue;
                for (int j = end.z - distance; j <= end.z + distance; j++)
                {
                    if (j < 0 || j >= hexes.GetLength(1)) continue;
                    if (Distance(hexes[i, j], end) == distance)
                    {
                        hexesAtThisDistance.Add(hexes[i, j]);
                    }
                }
            }

            List<List<Hex>> shortestFlights = new List<List<Hex>>();
            int shortestFlightCount = 0;
            bool flightFound = false;

            foreach (Hex hex in hexesAtThisDistance)
            {
                var flight = FindPath(start, hex);

                if (flight == null)
                {
                    continue;
                }
                else if (flight.Count > maxMove + 1)
                {
                    continue;
                }
                else if (!flightFound)
                {
                    flightFound = true;
                    shortestFlights.Add(flight);
                    shortestFlightCount = flight.Count;
                }
                else if (flight.Count < shortestFlightCount)
                {
                    shortestFlights = new List<List<Hex>>() { flight };
                    shortestFlightCount = flight.Count;
                }
                else if (flight.Count == shortestFlightCount)
                {
                    shortestFlights.Add(flight);
                }
            }

            if (flightFound)
            {
                return shortestFlights[UnityEngine.Random.Range(0, shortestFlights.Count - 1)];
            }
        }

        Debug.Log("no flight found");
        return null;
    }


    public void OnRightClick(Vector3 clickPos)
    {
        Hex clickedHex = GetHex(clickPos);
        clickedHex.Flip();
    }


    public Hex GetHex(int x, int z)
    {
        if (x < 0 || z < 0 || x >= hexes.GetLength(0) || z >= hexes.GetLength(1))
            return default;
        else if (!hexes[x, z].isActive)
            return default;
        else
            return hexes[x, z];
    }


    public Hex GetHex(Vector3 worldPos)
    {
        WorldPosToCell(worldPos, out int x, out int z);
        return GetHex(x, z);
    }


    public void OutlineHex(Color color, Hex hex)
    {
        hex.SetColor(color);
        hex.SetOutline(new bool[] { true, true, true, true, true, true });
    }


    public void OutlineHexes(Color color, List<Hex> hexes)
    {
        foreach (Hex hex in hexes)
        {
            hex.SetColor(color);

            bool[] segmentsActive = new bool[6];
            segmentsActive[0] = !hexes.Contains(GetHex(hex.x, hex.z + 1));
            segmentsActive[1] = !hexes.Contains(GetHex(hex.x + 1, hex.z));
            segmentsActive[2] = !hexes.Contains(GetHex(hex.x + 1, hex.z - 1));
            segmentsActive[3] = !hexes.Contains(GetHex(hex.x, hex.z - 1));
            segmentsActive[4] = !hexes.Contains(GetHex(hex.x - 1, hex.z));
            segmentsActive[5] = !hexes.Contains(GetHex(hex.x - 1, hex.z + 1));

            hex.SetOutline(segmentsActive);
        }
    }


    public void HighlightPath(List<Hex> path)
    {
        Hex lastHex = path[path.Count - 1];
        List<Hex> notLastHex = new List<Hex>(path);
        notLastHex.Remove(lastHex);

        ClearAllHexColors();
        OutlineHex(Color.green, lastHex);
        OutlineHexes(Color.cyan, notLastHex);
    }


    public void HighlightPossibleMoves(Hex startHex, int range, Color color)
    {
        List<Hex> hexesToHighlight = new List<Hex>() { startHex };
        for (int i = 0; i < range; i++)
        {
            int nInList = hexesToHighlight.Count;
            for (int j = 0; j < nInList; j++)
            {
                foreach (Hex neighbour in pathfinder.LookUpNeighbours(hexesToHighlight[j]))
                {
                    if (!hexesToHighlight.Contains(neighbour))
                    {
                        hexesToHighlight.Add(neighbour);
                    }
                }
            }
        }

        OutlineHexes(color, hexesToHighlight);
    }


    public void HighlightRange(Hex startHex, int range, Color color)
    {
        List<Hex> hexesToHighlight = new List<Hex>() { startHex };
        for (int i = startHex.x - range; i <= startHex.x + range; i++)
        {
            if (i < 0 || i >= hexes.GetLength(0)) continue;
            for (int j = startHex.z - range; j <= startHex.z + range; j++)
            {
                if (j < 0 || j >= hexes.GetLength(1)) continue;
                if (hexes[i, j].isActive && Distance(hexes[i,j], startHex) <= range)
                {
                    hexesToHighlight.Add(hexes[i, j]);
                }
            }
        }

        OutlineHexes(color, hexesToHighlight);
    }


    public void ClearAllHexColors()
    {
        foreach (Hex hex in hexes) hex.SetColor(Color.white);
    }


    private void WorldPosToCell(Vector3 worldPos, out int x, out int z)
    {
        worldPos -= transform.position;
        worldPos /= scale;
        z = Mathf.FloorToInt(worldPos.z * 2f / Mathf.Sqrt(3f) + Mathf.Sqrt(3f) / 4f);
        x = Mathf.FloorToInt(worldPos.x - z / 2f + 0.5f);
    }


    public Vector3 CellWorldPos(int x, int z)
    {
        return new Vector3(x + z / 2f, 0f, z * Mathf.Sqrt(3f) / 2f) * scale + transform.position;
    }


    public Vector3 CellWorldPos(Hex hex)
    {
        return CellWorldPos(hex.x, hex.z);
    }


    public int Distance(Hex a, Hex b)
    {
        return pathfinder.HCost(a, b);
    }


    private void OnDrawGizmos()
    {
        //if (pathfinder != null)
        //{
        //    Gizmos.color = Color.blue;
        //    pathfinder.DrawNeighbourLinks();
        //}
    }
}

