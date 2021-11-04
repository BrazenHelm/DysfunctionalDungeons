using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionBox : MonoBehaviour
{
    [SerializeField] private GameObject box = null;
    [SerializeField] private GameObject buttonLayout = null;
    [SerializeField] private Text buttonText = null;
    [SerializeField] private GameObject noButtonLayout = null;
    [SerializeField] private Text noButtonText = null;
    private Text currentText = null;

    private InstructionBoxContents contents;
    

    public void DisplayNewContents(InstructionBoxContents newContents)
    {
        box.SetActive(true);
        contents = newContents;

        if (contents.progressCondition.type == ProgressCondition.Type.PRESS_CONTINUE)
        {
            buttonLayout.SetActive(true);
            noButtonLayout.SetActive(false);
            currentText = buttonText;
        }
        else
        {
            buttonLayout.SetActive(false);
            noButtonLayout.SetActive(true);
            currentText = noButtonText;
        }

        currentText.text = contents.text;
    }


    public void Close()
    {
        box.SetActive(false);
    }


    public void Continue()
    {
        box.SetActive(false);
        FindObjectOfType<TutorialController>().MoveNextStage();
    }
}
