using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InformationBoxContents", menuName = "Tutorial Objects/Information")]
public class InfoBoxContents : TutorialStage
{
    public string title;
    public Sprite image;
    [TextArea(10, 14)] public string text;
}
