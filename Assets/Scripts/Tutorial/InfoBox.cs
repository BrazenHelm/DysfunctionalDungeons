using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoBox : MonoBehaviour
{
    [SerializeField] private GameObject box = null;
    [SerializeField] private Image image = null;
    [SerializeField] private Text titleText = null;
    [SerializeField] private Text bodyText = null;

    private InfoBoxContents contents;

    
    public void DisplayNewContents(InfoBoxContents newContents)
    {
        box.SetActive(true);
        contents = newContents;

        image.sprite = contents.image;
        titleText.text = contents.title;
        bodyText.text = contents.text;
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
