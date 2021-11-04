using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharSelectHUD : MonoBehaviour
{
    [SerializeField] private Text nameText = null;
    [SerializeField] private Text[] attributeTexts = null;
    [SerializeField] private Image portrait = null;
    [SerializeField] private Slider hpBar = null;
    [SerializeField] private GameObject[] actionsUsed = null;
    [SerializeField] private GameObject[] actionsUnused = null;
    private GameObject hud = null;


    private float[] hpBarValues = new float[]
    {
        0.000f, 0.203f, 0.312f, 0.407f, 0.514f, 0.601f, 0.712f, 0.808f, 1.000f
    }; 


    private void Awake()
    {
        hud = transform.GetChild(0).gameObject;
    }


    public void OnSelect(UnitController character)
    {
        hud.SetActive(true);
        nameText.text = character.gameObject.name;
        attributeTexts[0].text = character.Fighter.Strength().ToString();
        attributeTexts[1].text = character.Mover.GetSpeed().ToString();
        attributeTexts[2].text = character.Fighter.Armour().ToString();
        portrait.sprite = character.portrait;
        hpBar.value = hpBarValues[Mathf.Min(character.Fighter.HP, 8)];
    }


    //public void UpdateHPText(int newHP)
    //{
    //    attributeTexts[0].text = newHP.ToString();
    //}


    public void OnDeselect()
    {
        hud.SetActive(false);
    }


    public void SetActionsLeft(int actions)
    {
        switch (actions)
        {
            case 0:
                actionsUnused[0].SetActive(false);
                actionsUnused[1].SetActive(false);
                actionsUsed[0].SetActive(true);
                actionsUsed[1].SetActive(true);
                break;

            case 1:
                actionsUnused[0].SetActive(false);
                actionsUnused[1].SetActive(true);
                actionsUsed[0].SetActive(true);
                actionsUsed[1].SetActive(false);
                break;

            case 2:
                actionsUnused[0].SetActive(true);
                actionsUnused[1].SetActive(true);
                actionsUsed[0].SetActive(false);
                actionsUsed[1].SetActive(false);
                break;
        }
    }
}
