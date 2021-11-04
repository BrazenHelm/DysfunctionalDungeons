using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour
{
    MeshRenderer tile;
    [SerializeField] private GameObject[] blockers = null;
    [SerializeField] private GameObject[] outline = null;
    private int blockerIndex = 0;
    GameObject trap;

    public bool isActive = true;

    public int x, z;

    // for pathfinding
    public bool isBlocked = false;      // has an obstacle in the hex
    public bool isOccupied = false;     // has a mover in the hex
    public int moveCost = 1;
    public int gCost, hCost, fCost;
    public Hex prevHex;

    public int trapValue = 0;

    public UnitController unit = null;

    public bool Walkable { get { return isActive && !isBlocked && !isOccupied; } }


    private void Awake()
    {
        blockerIndex = Random.Range(0, blockers.Length);

        tile = transform.GetChild(0).GetComponent<MeshRenderer>();
        trap = transform.GetChild(2).gameObject;

        if (!isActive) tile.material.color = Color.black;
        else
        {
            blockers[blockerIndex].SetActive(isBlocked);
            trap.SetActive(trapValue != 0);
            tile.material.color = (isBlocked) ? Color.red : Color.white;
        }
    }


    private void Start()
    {
        if (!isActive)
        {
            SetOutline(new bool[] { false, false, false, false, false, false });
        }
    }


    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }


    public void Flip()
    {
        if (!isActive) return;
        isBlocked = !isBlocked;
        tile.material.color = (isBlocked) ? Color.red : Color.white;
        blockers[blockerIndex].SetActive(isBlocked);
    }


    public void SetColor(Color color)
    {
        if (!isActive) return;

        Color colorToSet = color;
        if (color == Color.white)
        {
            colorToSet = (isBlocked) ? Color.red : Color.white;
            if (colorToSet == Color.white) SetOutline(new bool[] { false, false, false, false, false, false });
            else SetOutline(new bool[] { true, true, true, true, true, true });
        }

        //if (color == Color.white)
        //    tile.material.color = (isBlocked) ? Color.red : Color.white;
        //else
        //    tile.material.color = color;

        foreach (GameObject segment in outline)
        {
            segment.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = colorToSet;
            segment.transform.GetChild(1).GetComponent<MeshRenderer>().material.color = colorToSet;
            segment.transform.GetChild(2).GetComponent<MeshRenderer>().material.color = colorToSet;
        }
        
    }


    public void SetOutline(bool[] segmentsActive)
    {
        if (segmentsActive.Length != 6)
        {
            Debug.LogError("Hex.SetOutline() : unexpected array size");
            return;
        }

        outline[0].SetActive(segmentsActive[0]);
        outline[1].SetActive(segmentsActive[1]);
        outline[2].SetActive(segmentsActive[2]);
        outline[3].SetActive(segmentsActive[3]);
        outline[4].SetActive(segmentsActive[4]);
        outline[5].SetActive(segmentsActive[5]);
    }


    public override string ToString()
    {
        return "(" + x.ToString() + "," + z.ToString() + ")";
    }
}
