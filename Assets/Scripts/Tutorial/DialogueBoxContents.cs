using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DialogueBoxContents", menuName = "Tutorial Objects/Dialogue")]
public class DialogueBoxContents : TutorialStage
{
    public DialogueCharacter speaker;
    [TextArea(10, 14)] public string text;
    public bool speakerOnRight;
}
