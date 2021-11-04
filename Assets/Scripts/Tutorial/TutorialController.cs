using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TutorialController : MonoBehaviour
{
    [SerializeField] private ProgressCondition continueCondition = null;
    [SerializeField] private TutorialStage[] tutorialMainStages = null;
    [SerializeField] private TutorialStage[] victoryDialogue = null;
    [SerializeField] private bool lastLevel = false;
    private int currentMainStage = -1;
    private int currentVictoryStage = -1;
    private bool doingVictoryStuff = false;
    private TutorialStage CurrentStage() { return tutorialMainStages[currentMainStage]; }

    private DialogueBox dialogueBox;
    private InfoBox infoBox;
    private InstructionBox instructionBox;
    public ProgressCondition Condition { get; private set; }
    public bool HasControl() { return Condition != null && Condition.type != ProgressCondition.Type.WAIT_FOR_TRIGGER; }
    public bool Waiting() { return Condition != null & Condition.type == ProgressCondition.Type.WAIT_FOR_TRIGGER; }


    private void Awake()
    {
        dialogueBox = FindObjectOfType<DialogueBox>();
        infoBox = FindObjectOfType<InfoBox>();
        instructionBox = FindObjectOfType<InstructionBox>();
    }


    void Start()
    {
        MoveNextStage();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (CurrentStage().GetType() == typeof(DialogueBoxContents))
            {
                dialogueBox.OnSpacePressed();
            }
        }
    }


    public void MoveNextStage()
    {
        if (!doingVictoryStuff)
        {
            if (++currentMainStage < tutorialMainStages.Length)
            {
                Debug.Log("Starting Tutorial Stage " + currentMainStage);

                TutorialStage stage = CurrentStage();

                if (stage.GetType() == typeof(DialogueBoxContents))
                {
                    dialogueBox.DisplayNewContents((DialogueBoxContents)stage);
                    infoBox.Close();
                    instructionBox.Close();
                    Condition = continueCondition;
                }
                else if (stage.GetType() == typeof(InfoBoxContents))
                {
                    dialogueBox.Close();
                    infoBox.DisplayNewContents((InfoBoxContents)stage);
                    instructionBox.Close();
                    Condition = continueCondition;
                }
                else if (stage.GetType() == typeof(InstructionBoxContents))
                {
                    dialogueBox.Close();
                    infoBox.Close();
                    instructionBox.DisplayNewContents((InstructionBoxContents)stage);
                    Condition = ((InstructionBoxContents)stage).progressCondition;
                }
                else if (stage.GetType() == typeof(NullTutorialStage))
                {
                    dialogueBox.Close();
                    infoBox.Close();
                    instructionBox.Close();
                    Condition = ((NullTutorialStage)stage).progressCondition;
                }
            }
            else
            {
                dialogueBox.Close();
                infoBox.Close();
                instructionBox.Close();
                Condition = null;

                if (lastLevel)
                {
                    FindObjectOfType<CreditsUI>().Open();
                }
            }
        }
        else
        {
            if (++currentVictoryStage < victoryDialogue.Length)
            {
                Debug.Log("Starting Victory Dialogue" + currentVictoryStage);

                TutorialStage stage = victoryDialogue[currentVictoryStage];

                if (stage.GetType() == typeof(DialogueBoxContents))
                {
                    dialogueBox.DisplayNewContents((DialogueBoxContents)stage);
                    infoBox.Close();
                    instructionBox.Close();
                    Condition = continueCondition;
                }
                else if (stage.GetType() == typeof(InfoBoxContents))
                {
                    dialogueBox.Close();
                    infoBox.DisplayNewContents((InfoBoxContents)stage);
                    instructionBox.Close();
                    Condition = continueCondition;
                }
                else if (stage.GetType() == typeof(InstructionBoxContents))
                {
                    dialogueBox.Close();
                    infoBox.Close();
                    instructionBox.DisplayNewContents((InstructionBoxContents)stage);
                    Condition = ((InstructionBoxContents)stage).progressCondition;
                }
                else if (stage.GetType() == typeof(NullTutorialStage))
                {
                    dialogueBox.Close();
                    infoBox.Close();
                    instructionBox.Close();
                    Condition = ((NullTutorialStage)stage).progressCondition;
                }
            }
            else
            {
                dialogueBox.Close();
                infoBox.Close();
                instructionBox.Close();
                Condition = null;

                if (lastLevel)
                {
                    FindObjectOfType<CreditsUI>().Open();
                }
            }
        }

        
    }


    public bool HasVictoryDialogue()
    {
        if (!doingVictoryStuff)
        {
            doingVictoryStuff = true;
            MoveNextStage();
        }
        return (victoryDialogue.Length > 0 && currentVictoryStage < victoryDialogue.Length);
    }
}
