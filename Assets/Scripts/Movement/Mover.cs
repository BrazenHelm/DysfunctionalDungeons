using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] private int hexesPerMove = 3; // hexes in one move
    public bool CanMove(int hexes) { return hexesPerMove >= hexes; }
    public bool CanDash(int hexes) { return hexesPerMove * 2 >= hexes; }

    public int GetSpeed() { return hexesPerMove; }

    private List<Vector3> path = null;
    private float posOnPath = 0f;

    public Fighter Fighter { get; private set; }

    [SerializeField] private float speed = 2f;   // hexes per second

    Animator animator = null;
    public HexGrid HexGrid { get; private set; }
    public Hex CurrentHex { get; private set; }
    CameraController camControl = null;

    private Queue<int> trapsOnPath;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        HexGrid = FindObjectOfType<HexGrid>();
        camControl = FindObjectOfType<CameraController>();
        trapsOnPath = new Queue<int>();
        Fighter = GetComponent<Fighter>();
    }


    public void Initialise()
    {
        UpdateHex();
        //animator.SetFloat("Speed", speed / 0.6f);
    }


    private void Update()
    {
        if (path == null)
        {
            animator.SetBool("Walking", false);
            return;
        }

        int hexLastFrame = Mathf.FloorToInt(posOnPath);
        posOnPath += speed * Time.deltaTime;
        int hexThisFrame = Mathf.FloorToInt(posOnPath);

        if (hexLastFrame != hexThisFrame)
        {
            int trapValue = trapsOnPath.Dequeue();
            if (trapValue > 0)
            {
                //Fighter.TakeHit(trapValue);
                Fighter.TakeHit(trapValue);
                //Debug.Log("walked over a trap for " + trapValue + " damage.");
            }
        }

        if (posOnPath >= path.Count - 1)
        {
            path = null;
            camControl.StopFollowing();
            GetComponent<UnitController>().DoneWithAction();
            return;
        }

        float t = posOnPath - hexThisFrame;

        transform.position = path[hexThisFrame] * (1 - t) + path[hexThisFrame + 1] * t;
        transform.LookAt(path[hexThisFrame + 1]);   
    }


    public void FollowPath(List<Hex> pathToFollow)
    {
        if (path != null)
        {
            Debug.LogWarning("Cannot set a new path until current path is done");
            return;
        }

        //if (pathToFollow.Count > moveSpeed + 1)
        //{
        //    Debug.Log("Attempted movement is too far for one action");
        //    return;
        //}

        CurrentHex.unit = null;
        CurrentHex.isOccupied = false;
        CurrentHex = pathToFollow[pathToFollow.Count - 1];
        CurrentHex.unit = GetComponent<UnitController>();
        CurrentHex.isOccupied = true;

        path = new List<Vector3>();
        for (int i = 0; i < pathToFollow.Count; i++)
        {
            path.Add(HexGrid.CellWorldPos(pathToFollow[i]));
            if (i != 0) trapsOnPath.Enqueue(pathToFollow[i].trapValue);
        }
        posOnPath = 0f;
        camControl.Follow(this);
        animator.SetBool("Walking", true);
    }


    private void UpdateHex()
    {
        CurrentHex = HexGrid.GetHex(transform.position);
        CurrentHex.unit = GetComponent<UnitController>();
        CurrentHex.isOccupied = true;
    }


    public void HighlightPossibleMoves(int actions)
    {
        HexGrid.ClearAllHexColors();

        while (actions > 0)
        {
            HexGrid.HighlightPossibleMoves(CurrentHex, hexesPerMove * actions,
                (actions % 2 == 0) ? Color.yellow : Color.magenta);

            actions--;
        }
    }


    public void Die()
    {
        CurrentHex.isOccupied = false;
        CurrentHex.unit = null;
        CurrentHex = null;
    }
}
