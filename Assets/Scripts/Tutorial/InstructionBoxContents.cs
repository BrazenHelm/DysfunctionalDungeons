using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InstructionBoxContents", menuName = "Tutorial Objects/Instructions")]
public class InstructionBoxContents : TutorialStage
{
    [TextArea(10,14)] public string text;
    public ProgressCondition progressCondition;
}
