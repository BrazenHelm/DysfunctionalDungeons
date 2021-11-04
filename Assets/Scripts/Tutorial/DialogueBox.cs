using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    [SerializeField] private GameObject box = null;
    [SerializeField] private DialogueBoxLayout leftLayout = null;
    [SerializeField] private DialogueBoxLayout rightLayout = null;
    private DialogueBoxLayout currentLayout = null;

    [SerializeField] private float charsPerSecond = 10.0f;
    private bool writing = false;
    private float timeSpentWriting = 0.0f;

    private DialogueBoxContents contents;


    private void Update()
    {
        if (writing)
        {
            timeSpentWriting += Time.deltaTime;
            int charsComplete = Mathf.FloorToInt(timeSpentWriting * charsPerSecond);
            charsComplete = Mathf.Min(charsComplete, contents.text.Length);
            currentLayout.SetSpeech(contents.text.Substring(0, charsComplete));
        }
    }


    public void DisplayNewContents(DialogueBoxContents newContents)
    {
        box.SetActive(true);
        contents = newContents;

        //Debug.Log("displaying: " + contents.name);

        if (contents.speakerOnRight)
            EnableRightLayout();
        else
            EnableLeftLayout();

        currentLayout.SetCharacter(
            contents.speaker.characterPortrait,
            contents.speaker.characterName);

        timeSpentWriting = 0.0f;
        writing = true;
    }


    public void Close()
    {
        box.SetActive(false);
        writing = false;
    }


    public void OnSpacePressed()
    {
        if (writing)
        {
            SkipTextWriting();
        }
        else
        {
            Continue();
        }
    }


    public void Continue()
    {
        Close();
        FindObjectOfType<TutorialController>().MoveNextStage();
    }


    public void SkipTextWriting()
    {
        currentLayout.SetSpeech(contents.text);
        writing = false;
    }


    private void EnableLeftLayout()
    {
        leftLayout.gameObject.SetActive(true);
        rightLayout.gameObject.SetActive(false);
        currentLayout = leftLayout;
    }


    private void EnableRightLayout()
    {
        rightLayout.gameObject.SetActive(true);
        leftLayout.gameObject.SetActive(false);
        currentLayout = rightLayout;
    }
}
