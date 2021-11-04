using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAIType : MonoBehaviour
{
    protected UnitController controller;
    protected Fighter fighter;
    protected Mover mover;
    protected HexGrid hexGrid;

    //public bool doingTurn = false;

    private void Awake()
    {
        controller = GetComponent<UnitController>();
        fighter = GetComponent<Fighter>();
        mover = GetComponent<Mover>();
        hexGrid = FindObjectOfType<HexGrid>();
    }


    //private void StartTurn() { doingTurn = true; }
    protected void EndTurn()
    {
        //doingTurn = false;
        //Debug.Log("ending turn");
        //controller.DoneWithTurn();
    }

    public abstract IEnumerator DoTurn(List<UnitController> playerUnits, int mostRecentActor);
}
