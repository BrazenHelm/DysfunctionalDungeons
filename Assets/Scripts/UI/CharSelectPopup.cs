using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharSelectPopup : MonoBehaviour
{
    [SerializeField] private Text nameText = null;
    [SerializeField] private Text hpText = null;
    [SerializeField] private GameObject actionBar1 = null;
    [SerializeField] private GameObject actionBar2 = null;
    private GameObject popup;

    private int actionsLeft = 2;

    private void Awake()
    {
        popup = transform.GetChild(0).gameObject;
    }


    //private void Start()
    //{
    //    popup.SetActive(false);
    //}


    public void OnSelect(UnitController character)
    {
        popup.SetActive(true);
        nameText.text = character.gameObject.name;
        hpText.text = character.Fighter.HP.ToString();
        GetComponent<AnchorUI>().anchor = character.transform;
        //SetActionsLeft(2);
    }


    public void UpdateHPText(int newHP)
    {
        hpText.text = newHP.ToString();
    }


    public void SetActionsLeft(int nActions)
    {
        actionsLeft = nActions;

        if (actionsLeft <= 1)
            actionBar2.SetActive(false);
        else
            actionBar2.SetActive(true);

        if (actionsLeft <= 0)
            actionBar1.SetActive(false);
        else
            actionBar1.SetActive(true);
    }


    public void OnDeselect()
    {
        popup.SetActive(false);
    }

}
