using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum Ability { FARMER_WOBBLY_PITCHFORK, NONE };


public class UnitController : MonoBehaviour
{
    public bool DoingTurn { get; private set; }
    public void DoneWithTurn() { DoingTurn = false; }

    private bool busyActing = false;
    public void DoneWithAction() { busyActing = false; }

    public Mover Mover { get; private set; }
    public Fighter Fighter { get; private set; }

    [SerializeField] private Ability[] abilities = null;

    [SerializeField] private SkinnedMeshRenderer model = null;
    private const float DEATH_FADEOUT_TIME = 5f;
    private float timeSinceDeath = 0f;
    private bool dying = false;

    public int ActionsLeft { get; private set; } = 2;
    public Sprite portrait;

    private IAction queuedAction = null;
    public bool ActionQueued() { return queuedAction != null; }

    [SerializeField] private bool isEnemy = false;
    public bool IsEnemy() { return isEnemy; }
    private EnemyAIType enemyAI = null;

    [SerializeField] private CharaterActionsUI actionButtons = null;
    [SerializeField] private AbilityCooldown abilityCooldown = null;


    private void Awake()
    {
        Mover = GetComponent<Mover>();
        Fighter = GetComponent<Fighter>();
        if (isEnemy) enemyAI = GetComponent<EnemyAIType>();
    }


    private void Update()
    {
        if (Fighter.dying)
        {
            Material material = model.material;
            material.SetOverrideTag("RenderType", "Transparent");
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            dying = true;
        }
        if (dying)
        {
            timeSinceDeath += Time.deltaTime;

            if (timeSinceDeath >= DEATH_FADEOUT_TIME)
            {
                Destroy(gameObject);
            }
            else
            {
                Color color = model.material.color;
                color.a = 1.0f - timeSinceDeath / DEATH_FADEOUT_TIME;
                model.material.color = color;
            }
        }
    }


    public void StartTurn()
    {
        var unitDialogueTrigger = GetComponent<UnitDialogueTrigger>();
        if (unitDialogueTrigger != null) unitDialogueTrigger.OnTurnStart();

        if (isEnemy) return;

        DoingTurn = true;
        Fighter.selected = true;
        ActionsLeft = 2;
        queuedAction = null;
        FindObjectOfType<CharSelectHUD>().SetActionsLeft(2);

        actionButtons.Show();
        if (abilityCooldown != null) abilityCooldown.OnTurnStart();
    }


    public void StartTurn(List<UnitController> playerUnits, int mostRecentActor)
    {
        if (!isEnemy) return;

        DoingTurn = true;
        ActionsLeft = 2;
        queuedAction = null;
        StartCoroutine(enemyAI.DoTurn(playerUnits, mostRecentActor));
    }


    public void QueueMovement(List<Hex> path)
    {
        queuedAction = new Movement(Mover, path);
        if (!queuedAction.Possible(ActionsLeft))
        {
            queuedAction = new Dash(Mover, path);
            if (!queuedAction.Possible(ActionsLeft))
            {
                queuedAction = null;
                return;
            }
        }

        Mover.HexGrid.HighlightPath(path);
    }


    public void QueueAttack(Fighter victim)
    {
        queuedAction = new Attack(Fighter, victim);
        if (!queuedAction.Possible(ActionsLeft))
        {
            queuedAction = null;
        }
        else
        {
            Mover.HexGrid.ClearAllHexColors();
            Mover.HexGrid.OutlineHex(Color.blue, victim.Mover.CurrentHex);
        }
    }


    public void QueueAbility(int index, Hex targetHex, UnitController targetUnit)
    {
        Ability abilityType = abilities[index];
        switch (abilityType)
        {
            case Ability.FARMER_WOBBLY_PITCHFORK:
                if (targetUnit != null)
                {
                    queuedAction = new FarmerWobblyPitchfork(Fighter, targetUnit.Fighter);
                    if (!queuedAction.Possible(ActionsLeft))
                    {
                        queuedAction = null;
                    }
                    else
                    {
                        Mover.HexGrid.ClearAllHexColors();
                        Mover.HexGrid.OutlineHex(Color.blue, targetHex);
                    }
                }
                break;

            default:
                break;
        }
    }


    public IEnumerator PerformQueuedAction()
    {
        if (queuedAction == null)
        {
            Debug.LogWarning("No action currently queued");
            yield break;
        }

        if (actionButtons != null) actionButtons.Hide();

        Mover.HexGrid.ClearAllHexColors();

        int actionsUsed = queuedAction.Perform();
        if (actionsUsed == -1) ActionsLeft = 0;
        else ActionsLeft -= actionsUsed;

        FindObjectOfType<CharSelectHUD>().SetActionsLeft(ActionsLeft);

        busyActing = true;
        queuedAction = null;

        while (busyActing)
        {
            yield return null;
        }

        if (actionButtons != null) actionButtons.Show();

        if (ActionsLeft == 0)
        {
            Mover.HexGrid.ClearAllHexColors();
            DoingTurn = false;
            Fighter.selected = false;
            if (actionButtons != null) actionButtons.Hide();
        }

        var tutControl = FindObjectOfType<TutorialController>();
        if (tutControl.HasControl() && tutControl.Condition.type == ProgressCondition.Type.ACTION_COMPLETE)
        {
            tutControl.MoveNextStage();
        }

        FindObjectOfType<GameController>().SetMode(0);
    }


    public void DisplayMovementRanges()
    {
        Mover.HighlightPossibleMoves(ActionsLeft);
    }


    public void DisplayAttackRange()
    {
        Mover.HexGrid.ClearAllHexColors();
        Mover.HexGrid.HighlightRange(Mover.CurrentHex, Fighter.Range(), Color.cyan);
    }


    public void DisplayAbilityRange()
    {
        Mover.HexGrid.ClearAllHexColors();

        switch (abilities[0])
        {
            case Ability.FARMER_WOBBLY_PITCHFORK:
                Mover.HexGrid.HighlightRange(Mover.CurrentHex, Fighter.Range() + 2, Color.cyan);
                break;

            default:
                break;
        }
    }


    //public void HideButtons()
    //{
    //    actionButtons.Hide();
    //}


    public void CancelQueuedAction()
    {
        queuedAction = null;
    }
}
