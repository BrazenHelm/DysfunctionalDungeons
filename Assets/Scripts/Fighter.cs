using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fighter : MonoBehaviour
{
    private Animator animator;
    private AudioController audioController;
    private Fighter currentOpponent;
    public Mover Mover { get; private set; }

    [SerializeField] private int maxHP = 10;
    [SerializeField] private int armour = 0;
    [SerializeField] private int strength = 3;
    [SerializeField] private int range = 1;
    [SerializeField] private float attackDelay = 1f; //time in seconds between attack starting and hit connecting
    public int HP { get; private set; }

    public int Strength() { return strength; }
    public int Range() { return range; }
    public int Armour() { return armour; }

    public bool dying = false;

    public bool selected = false;

    [SerializeField] private Slider hpBar = null;
    [SerializeField] private GameObject damageTextPrefab = null;
    [SerializeField] private AbilityCooldown abilityCooldown = null;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioController = GetComponent<AudioController>();
        Mover = GetComponent<Mover>();
        HP = maxHP;
    }


    private void Start()
    {
        if (hpBar) hpBar.value = 1;
    }


    public void Attack(Fighter victim, bool isAbility)
    {
        if (victim.dying) return;
        currentOpponent = victim;

        transform.LookAt(victim.transform);

        if (isAbility)
        {
            audioController.PlayAbilityClip();
            animator.SetTrigger("Ability");
            if (abilityCooldown != null) abilityCooldown.GoOnCooldown();
        }
        else
        {
            audioController.PlayAttackClip();
            animator.SetTrigger("Attack");
        }

        StartCoroutine(DoAttack(victim, isAbility));
    }


    public bool CanAttack(Fighter other, int rangeMod)
    {
        if (other.dying) return false;
        else if (other == this) return false;
        else return (Mover.HexGrid.Distance(Mover.CurrentHex, other.Mover.CurrentHex) <= range + rangeMod);
    }


    private IEnumerator DoAttack(Fighter victim, bool extraDelay)
    {
        yield return new WaitForSeconds(attackDelay + ((extraDelay) ? 1.0f : 0.0f));

        victim.TakeHit(strength);
        GetComponent<UnitController>().DoneWithAction();
    }

    //// Animation Event
    //private void Hit()
    //{
    //    currentOpponent.TakeHit(strength);
    //}


    //// Animation Event
    //private void Shoot()
    //{
    //    Hit();
    //}


    public void TakeHit(int damage)
    {
        if (dying) return;

        damage -= armour;
        HP -= damage;
        animator.SetTrigger("Injured");

        GameObject damageText = Instantiate(damageTextPrefab, transform.Find("Character UI Canvas"));
        damageText.transform.SetAsLastSibling();
        damageText.GetComponent<AnchorUI>().anchor = transform;
        damageText.GetComponent<DamageText>().SetValue(damage);


        var unitDialogueTrigger = GetComponent<UnitDialogueTrigger>();
        if (unitDialogueTrigger != null) unitDialogueTrigger.OnHPValueChanged(HP);


        if (hpBar)
        {
            hpBar.value = (float)HP / maxHP;
        }

        //if (selected)
        //{
        //    //FindObjectOfType<CharSelectPopup>().UpdateHPText(HP);
        //    //FindObjectOfType<CharSelectHUD>().UpdateHPText(HP);
        //}

        if (HP <= 0) Die();
        else audioController.PlayHitClip();
    }


    private void Die()
    {
        animator.SetTrigger("Die");
        dying = true;
        Mover.Die();
        audioController.PlayDeathClip();
        hpBar.gameObject.SetActive(false);
    }
}
