using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;


public enum Mode { MOVE, ATTACK, ABILITY, NONE };

public class GameController : MonoBehaviour
{
    private HexGrid hexGrid = null;
    private Plane gridPlane;

    private TutorialController tutorial = null;
    private CameraController camControl = null;
    //private CharSelectPopup charSelectPopup = null;
    private CharSelectHUD charSelectHUD = null;
    private InitiativeUI initiativeUI = null;
    [SerializeField] AnchorUI confirmActionUI = null;

    [SerializeField] List<UnitController> playerUnits = null;
    [SerializeField] List<UnitController> enemyUnits = null;

    private int selectedUnit = 0;
    private int selectedEnemy = -1;

    private Mode mode = Mode.NONE;

    private int currentPlayerInitiative = 0;
    private int currentEnemyInitiative = -1;
    private bool isPlayerTurn = true;

    private bool levelDone = false;
    private bool gamePaused = false;
    private bool betweenTurns = false;

    [SerializeField] private float EndOfTurnDelay = 2f; // in seconds

    [SerializeField] private bool autoVictory = false;
    [SerializeField] private bool autoDefeat = false;
    [SerializeField] private bool bossLevel = false;


    private void Awake()
    {
        hexGrid = FindObjectOfType<HexGrid>();
        gridPlane = new Plane(Vector3.up, hexGrid.transform.position);
        tutorial = GetComponent<TutorialController>();
        camControl = FindObjectOfType<CameraController>();
        //charSelectPopup = FindObjectOfType<CharSelectPopup>();
        charSelectHUD = FindObjectOfType<CharSelectHUD>();
        initiativeUI = FindObjectOfType<InitiativeUI>();
        confirmActionUI.gameObject.SetActive(false);
    }


    private void Start()
    {
        if (autoVictory)
        {
            StartCoroutine(OnVictory());
            return;
        }

        if (autoDefeat)
        {
            StartCoroutine(OnDefeat());
            return;
        }

        MusicPlayer musicPlayer = FindObjectOfType<MusicPlayer>();
        if (musicPlayer == null) Debug.Log("no music player found");
        else if (bossLevel) musicPlayer.PlayTrack(MusicPlayer.Track.BOSS);
        else musicPlayer.PlayTrack(MusicPlayer.Track.TUTORIAL);
        //FindObjectOfType<MusicPlayer>().PlayTrack(MusicPlayer.Track.TUTORIAL);

        foreach (UnitController unit in playerUnits)
        {
            unit.Mover.Initialise();
        }
        foreach (UnitController unit in enemyUnits)
        {
            unit.Mover.Initialise();
        }
        if (!bossLevel) StartCoroutine(StartInitiative());
    }


    private void Update()
    {
        if (levelDone) return;

        // First of all we check to see if any units are dead since last frame
        //bool removedAnyUnits = false;

        for (int i = playerUnits.Count - 1; i >= 0; i--)
        {
            if (playerUnits[i].Fighter.dying)
            {
                if (i == selectedUnit && isPlayerTurn)
                {
                    StartCoroutine(MoveToNextInitiative());
                }
                playerUnits.RemoveAt(i);
                
                if (i < selectedUnit)
                {
                    selectedUnit--;
                    currentPlayerInitiative--;
                }

                if (playerUnits.Count == 0)
                {
                    StartCoroutine(OnDefeat());
                    return;
                }

                RedoInitiativeUI();
            }
        }

        for (int j = enemyUnits.Count - 1; j >= 0; j--)
        {
            if (enemyUnits[j].Fighter.dying)
            {
                if (j == selectedEnemy && !isPlayerTurn)
                {
                    StartCoroutine(MoveToNextInitiative());
                }
                enemyUnits.RemoveAt(j);
                
                if (j < selectedEnemy)
                {
                    selectedEnemy--;
                    currentEnemyInitiative--;
                }

                if (enemyUnits.Count == 0)
                {
                    StartCoroutine(OnVictory());
                    return;
                }

                RedoInitiativeUI();
            }
        }


        // Next we look for cancelling actions or pausing the game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!tutorial.HasControl())
            {
                if (playerUnits[selectedUnit].DoingTurn && playerUnits[selectedUnit].ActionQueued())
                {
                    CancelAction();
                }
                else if (playerUnits[selectedUnit].DoingTurn && mode != Mode.NONE)
                {
                    SetMode(0);
                }
                else if (!gamePaused)
                {
                    PauseGame();
                }
            }
            else if (!gamePaused)
            {
                PauseGame();
            }
        }

        if (gamePaused) return;

        // Finally we look for controls
        if (playerUnits[selectedUnit].DoingTurn)
        {
            // Left Click
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    PointerEventData ped = new PointerEventData(EventSystem.current);
                    ped.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    List<RaycastResult> results = new List<RaycastResult>();
                    EventSystem.current.RaycastAll(ped, results);

                    for (int i = 0; i < results.Count; i++)
                    {
                        if (results[i].gameObject.GetComponent<AllowClickThruUI>() == null)
                        {
                            return;
                        }
                    }
                }

                if (tutorial.HasControl())
                {
                    if (tutorial.Condition.type != ProgressCondition.Type.TARGET_HEX)
                    {
                        return;
                    }
                }

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (gridPlane.Raycast(ray, out float distance))
                {
                    Vector3 hitPoint = ray.GetPoint(distance);
                    Hex clickedHex = hexGrid.GetHex(hitPoint);
                    UnitController clickedUnit = GetUnit(clickedHex);

                    if (tutorial.HasControl())
                    {
                        int x = clickedHex.x;
                        int z = clickedHex.z;
                        bool acceptable = false;

                        for (int i = 0; i < tutorial.Condition.paramaters.Length; i += 2)
                        {
                            if (tutorial.Condition.paramaters[i] == x && tutorial.Condition.paramaters[i+1] == z)
                            {
                                acceptable = true;
                                break;
                            }
                        }
                        if (acceptable)
                        {
                            tutorial.MoveNextStage();
                        }
                        else
                        {
                            return;
                        }
                    }

                    // clicked on an empty hex: move there
                    if (clickedUnit == null && mode == Mode.MOVE)
                    {
                        var path = hexGrid.FindPath(
                            playerUnits[selectedUnit].Mover.CurrentHex,
                            clickedHex);

                        if (path != null)
                        {
                            playerUnits[selectedUnit].QueueMovement(path);
                            confirmActionUI.gameObject.SetActive(true);
                            confirmActionUI.anchor = clickedHex.transform;
                        }
                    }
                    // clicked on a hex containing another unit: attack them
                    else if (clickedUnit.IsEnemy() && mode == Mode.ATTACK)
                    {
                        playerUnits[selectedUnit].QueueAttack(clickedUnit.Fighter);
                        confirmActionUI.gameObject.SetActive(true);
                        confirmActionUI.anchor = clickedUnit.transform;
                    }
                    else if (clickedUnit.IsEnemy() && mode == Mode.ABILITY)
                    {
                        playerUnits[selectedUnit].QueueAbility(0, clickedHex, clickedUnit);
                        confirmActionUI.gameObject.SetActive(true);
                        confirmActionUI.anchor = clickedUnit.transform;
                    }
                }
            }
        }
        else if (!isPlayerTurn && !betweenTurns && !enemyUnits[selectedEnemy].DoingTurn)
        {
            StartCoroutine(MoveToNextInitiative());
        }
    }


    private IEnumerator StartInitiative()
    {
        initiativeUI.SetUnits(
            playerUnits[0],
            enemyUnits[0],
            playerUnits[1 % playerUnits.Count],
            enemyUnits[1 % enemyUnits.Count]
        );

        //Debug.Log("pi = " + currentPlayerInitiative + ", ei = " + currentEnemyInitiative);

        yield return new WaitForSeconds(1.0f);

        SelectUnit(0);
        playerUnits[0].StartTurn();
    }


    private IEnumerator MoveToNextInitiative()
    {
        while (gamePaused || (tutorial.HasControl() && tutorial.Condition.type == ProgressCondition.Type.PRESS_CONTINUE))
        {
            yield return null;
        }

        betweenTurns = true;
        yield return new WaitForSeconds(EndOfTurnDelay);
        betweenTurns = false;

        //Debug.Log("moving to next initiative");
        isPlayerTurn = !isPlayerTurn;
        //Debug.Log("pi = " + currentPlayerInitiative + ", ei = " + currentEnemyInitiative);


        if (isPlayerTurn)
        {
            currentPlayerInitiative++;
            currentPlayerInitiative %= playerUnits.Count;
            //Debug.Log("pi = " + currentPlayerInitiative + ", ei = " + currentEnemyInitiative);
            SelectUnit(currentPlayerInitiative);
            initiativeUI.MoveAlong(enemyUnits[(currentEnemyInitiative + 2) % enemyUnits.Count]);

            playerUnits[selectedUnit].StartTurn();
            //charSelectPopup.SetActionsLeft(2);            
        }
        else
        {
            //if (enemyUnits.Count == 0)
            //{
            //    StartCoroutine(OnVictory());
            //    yield break;
            //}
            charSelectHUD.OnDeselect();

            currentEnemyInitiative++;
            currentEnemyInitiative %= enemyUnits.Count;
            Debug.Log("pi = " + currentPlayerInitiative + ", ei = " + currentEnemyInitiative);
            selectedEnemy = currentEnemyInitiative;
            camControl.PanTransitionTo(enemyUnits[selectedEnemy].transform.position);
            initiativeUI.MoveAlong(playerUnits[(currentPlayerInitiative + 2) % playerUnits.Count]);
            //DoAITurn();

            enemyUnits[selectedEnemy].StartTurn(playerUnits, currentPlayerInitiative);
        }
    }


    public void StartTurn()
    {
        playerUnits[selectedUnit].StartTurn();
        //Debug.Log("Starting turn of " + playerUnits[selectedUnit].name);
        //charSelectPopup.SetActionsLeft(2);
    }


    public void ConfirmAction()
    {
        if (tutorial.HasControl())
        {
            if (tutorial.Condition.type != ProgressCondition.Type.CONFIRM_ACTION)
            {
                return;
            }
            else
            {
                tutorial.MoveNextStage();
            }
        }

        StartCoroutine(DoActionPls());
        confirmActionUI.gameObject.SetActive(false);
    }


    private IEnumerator DoActionPls()
    {
        yield return playerUnits[selectedUnit].StartCoroutine(playerUnits[selectedUnit].PerformQueuedAction());
        int actionsLeft = playerUnits[selectedUnit].ActionsLeft;
        //charSelectPopup.SetActionsLeft(actionsLeft);
        if (actionsLeft <= 0)
        {
            //playerUnits[selectedUnit].HideButtons();
            StartCoroutine(MoveToNextInitiative());
        }
    }


    private IEnumerator OnVictory()
    {
        levelDone = true;

        while (GetComponent<TutorialController>().HasVictoryDialogue())
        {
            yield return null;
        }

        yield return new WaitForSeconds(EndOfTurnDelay);

        FindObjectOfType<EndLevelMenu>().OnVictory();
    }


    private IEnumerator OnDefeat()
    {
        levelDone = true;

        yield return new WaitForSeconds(EndOfTurnDelay);

        FindObjectOfType<EndLevelMenu>().OnDefeat();
    }


    public void NewRound()
    {
        return;
    }
    
     
    private void SelectUnit(int unitToSelect)
    {
        selectedUnit = unitToSelect;
        camControl.PanTransitionTo(playerUnits[unitToSelect].transform.position);
        charSelectHUD.OnSelect(playerUnits[unitToSelect]);
    }


    private UnitController GetUnit(Hex hex)
    {
        for (int i = 0; i < playerUnits.Count; i++)
        {
            if (playerUnits[i].Mover.CurrentHex == hex)
            {
                return playerUnits[i];
            }
        }
        for (int j = 0; j < enemyUnits.Count; j++)
        {
            if (enemyUnits[j].Mover.CurrentHex == hex)
            {
                return enemyUnits[j];
            }
        }
        return null;
    }


    public void SetMode(int modeIndex)
    {
        bool tutHasControl = false;

        if (tutorial.HasControl())
        {
            tutHasControl = true;

            if (tutorial.Condition.type != ProgressCondition.Type.SELECT_MODE)
            {
                return;
            }
            if (tutorial.Condition.paramaters[0] != modeIndex)
            {
                return;
            }
        }

        CancelAction();

        switch (modeIndex)
        {
            case 0:
                mode = Mode.NONE;
                hexGrid.ClearAllHexColors();
                break;

            case 1:
                mode = Mode.MOVE;
                playerUnits[selectedUnit].DisplayMovementRanges();
                break;

            case 2:
                mode = Mode.ATTACK;
                playerUnits[selectedUnit].DisplayAttackRange();
                break;

            case 3:
                mode = Mode.ABILITY;
                playerUnits[selectedUnit].DisplayAbilityRange();
                break;
        }

        if (tutHasControl)
        {
            tutorial.MoveNextStage();
        }
    }


    private void CancelAction()
    {
        playerUnits[selectedUnit].CancelQueuedAction();
        confirmActionUI.gameObject.SetActive(false);
    }


    public void PauseGame()
    {
        gamePaused = !gamePaused;

        if (gamePaused)
        {
            FindObjectOfType<PauseMenu>().Open();
        }

        var pausables = FindObjectsOfType<MonoBehaviour>().OfType<IPausable>();
        foreach (IPausable pausable in pausables)
        {
            if (gamePaused) pausable.Pause();
            else pausable.Unpause();
        }
    }


    private void RedoInitiativeUI()
    {
        if (isPlayerTurn)
        {
            initiativeUI.SetUnits(
                playerUnits[selectedUnit],
                enemyUnits[(selectedEnemy + 1) % enemyUnits.Count],
                playerUnits[(selectedUnit + 1) % playerUnits.Count],
                enemyUnits[(selectedEnemy + 2) % enemyUnits.Count]
            );
        }
        else
        {
            initiativeUI.SetUnits(
                enemyUnits[selectedEnemy],
                playerUnits[(selectedUnit + 1) % playerUnits.Count],
                enemyUnits[(selectedEnemy + 1) % enemyUnits.Count],
                playerUnits[(selectedUnit + 2) % playerUnits.Count]
            );
        }
    }
}
