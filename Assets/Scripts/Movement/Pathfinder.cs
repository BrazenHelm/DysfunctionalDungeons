// This class is a version of the one shown in Code Monkey's tutorial
// A* Pathfinding in Unity (https://www.youtube.com/watch?v=alU04hvz6L4)
// adapted for use with a hexagonal grid and moveable blockers


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pathfinder
{
    private int width, height;
    private Hex[,] hexes;

    // nodes being considered in A* algorithm
    private List<Hex> openList = null;

    // nodes already fully searched in A* algorithm
    private List<Hex> closedList = null;

    // dictionary of neighbours of each cell
    private Dictionary<Hex, List<Hex>> neighbourLookup = null;


    public Pathfinder(int width, int height, Hex[,] hexes)
    {
        this.width = width;
        this.height = height;
        this.hexes = hexes;

        neighbourLookup = new Dictionary<Hex, List<Hex>>();
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                Hex hex = hexes[x, z];
                neighbourLookup.Add(hex, GetNeighbours(hex));
            }
        }
    }

 
    public List<Hex> FindPath(int startX, int startZ, int endX, int endZ)
    {
        Hex start = hexes[startX, startZ];
        Hex end = hexes[endX, endZ];
        openList = new List<Hex>() { start };
        closedList = new List<Hex>();

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                Hex hex = hexes[x, z];
                hex.gCost = int.MaxValue;
                hex.CalculateFCost();
                hex.prevHex = null;
            }
        }

        start.gCost = 0;
        start.hCost = HCost(start, end);
        start.CalculateFCost();

        while (openList.Count > 0)
        {
            Hex current = GetLowestFCost(openList);
            if (current == end)
            {
                return CalculatePath(end);
            }
            openList.Remove(current);
            closedList.Add(current);

            foreach (Hex neighbour in neighbourLookup[current])
            {
                if (!neighbour.Walkable) closedList.Add(neighbour);
                if (closedList.Contains(neighbour)) continue;

                int gCost = current.gCost + neighbour.moveCost;
                if (gCost < neighbour.gCost)
                {
                    neighbour.prevHex = current;
                    neighbour.gCost = gCost;
                    neighbour.hCost = HCost(neighbour, end);
                    neighbour.CalculateFCost();

                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }
        }

        // out of nodes on openList
        return null;
    }


    public List<Hex> LookUpNeighbours(Hex hex)
    {
        List<Hex> neighbours = new List<Hex>(neighbourLookup[hex]);

        for (int i = neighbours.Count - 1; i >= 0; i--)
        {
            if (!neighbours[i].Walkable) neighbours.RemoveAt(i);
        }

        return neighbours;
    }


    private List<Hex> CalculatePath(Hex end)
    {
        List<Hex> path = new List<Hex>() { end };
        Hex current = end;
        while (current.prevHex != null)
        {
            path.Add(current.prevHex);
            current = current.prevHex;
        }
        path.Reverse();
        return path;
    }


    private List<Hex> GetNeighbours(Hex hex)
    {
        List<Hex> neighbours = new List<Hex>();

        if (hex.x > 0)
        {
            // left
            if (hexes[hex.x - 1, hex.z].isActive)
                neighbours.Add(hexes[hex.x - 1, hex.z]);

            if (hex.z < height - 1)
            {
                // upper left
                if (hexes[hex.x - 1, hex.z + 1].isActive)
                    neighbours.Add(hexes[hex.x - 1, hex.z + 1]);
            }
        }
        if (hex.x < width - 1)
        {
            // right
            if (hexes[hex.x + 1, hex.z].isActive)
                neighbours.Add(hexes[hex.x + 1, hex.z]);

            if (hex.z > 0)
            {
                // lower right
                if (hexes[hex.x + 1, hex.z - 1].isActive)
                    neighbours.Add(hexes[hex.x + 1, hex.z - 1]);
            }
        }
        if (hex.z > 0)
        {
            // lower left
            if (hexes[hex.x, hex.z - 1].isActive)
                neighbours.Add(hexes[hex.x, hex.z - 1]);
        }
        if (hex.z < height - 1)
        {
            // upper right
            if (hexes[hex.x, hex.z + 1].isActive)
                neighbours.Add(hexes[hex.x, hex.z + 1]);
        }

        return neighbours;
    }


    public int HCost(Hex a, Hex b)
    {
        int dx = a.x - b.x;
        int dz = a.z - b.z;

        if (dx * dz < 0)
        {
            return Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dz));
        }
        else
        {
            return Mathf.Abs(dx) + Mathf.Abs(dz);
        }
    }


    private Hex GetLowestFCost(List<Hex> hexes)
    {
        if (hexes.Count == 0) return null;

        Hex result = hexes[0];

        for (int i = 0; i < hexes.Count; i++)
        {
            if (hexes[i].fCost < result.fCost)
            {
                result = hexes[i];
            }
        }

        return result;
    }


    public void DrawNeighbourLinks()
    {
        foreach (var kvp in neighbourLookup)
        {
            foreach (Hex nb in kvp.Value)
            {
                Gizmos.DrawLine(kvp.Key.transform.position, nb.transform.position);
            }
        }
    }
}
