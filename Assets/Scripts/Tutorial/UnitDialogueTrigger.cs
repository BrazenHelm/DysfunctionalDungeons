using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class UnitDialogueTrigger : MonoBehaviour
{
    [SerializeField] private int triggerOnTurnStart = -1;
    //[SerializeField] private int turnStartTrigger = 0;
    private int turns = 0;

    [SerializeField] private int triggerOnHPValue = -1000;
    //[SerializeField] private int hpValueTrigger = 0;

    TutorialController tutorialController;


    private void Awake()
    {
        tutorialController = FindObjectOfType<TutorialController>();
    }


    public void OnTurnStart()
    {
        turns++;
        if (turns == triggerOnTurnStart)
        {
            if (tutorialController.Waiting())
                tutorialController.MoveNextStage();
        }
    }


    public void OnHPValueChanged(int value)
    {
        if (value <= triggerOnHPValue)
        {
            if (tutorialController.Waiting())
                tutorialController.MoveNextStage();
        }
    }
}



