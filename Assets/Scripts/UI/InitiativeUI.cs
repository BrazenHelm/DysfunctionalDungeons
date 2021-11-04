using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitiativeUI : MonoBehaviour
{
    [SerializeField] Image portraitTurn0 = null;
    [SerializeField] Image portraitTurn1 = null;
    [SerializeField] Image portraitTurn2 = null;
    [SerializeField] Image portraitTurn3 = null;

    UnitController unitTurn0;
    UnitController unitTurn1;
    UnitController unitTurn2;
    UnitController unitTurn3;

    CameraController camControl;


    private void Awake()
    {
        camControl = FindObjectOfType<CameraController>();
    }


    public void SetUnits(UnitController unit0, UnitController unit1, UnitController unit2, UnitController unit3)
    {
        unitTurn0 = unit0;
        unitTurn1 = unit1;
        unitTurn2 = unit2;
        unitTurn3 = unit3;
        UpdatePortraits();
    }


    public void MoveAlong(UnitController unit3)
    {
        unitTurn0 = unitTurn1;
        unitTurn1 = unitTurn2;
        unitTurn2 = unitTurn3;
        unitTurn3 = unit3;
        UpdatePortraits();
    }


    public void UpdatePortraits()
    {
        portraitTurn0.sprite = unitTurn0.portrait;
        portraitTurn1.sprite = unitTurn1.portrait;
        portraitTurn2.sprite = unitTurn2.portrait;
        portraitTurn3.sprite = unitTurn3.portrait;
    }


    public void OnClickPortrait(int index)
    {
        switch (index)
        {
            case 0:
                camControl.PanTransitionTo(unitTurn0.transform.position);
                break;

            case 1:
                camControl.PanTransitionTo(unitTurn1.transform.position);
                break;

            case 2:
                camControl.PanTransitionTo(unitTurn2.transform.position);
                break;

            case 3:
                camControl.PanTransitionTo(unitTurn3.transform.position);
                break;

            default:
                Debug.LogError("InitiativeUI::OnClickPortrait() - invalid index");
                break;

        }
    }
}
