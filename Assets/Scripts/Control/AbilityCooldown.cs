using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AbilityCooldown : MonoBehaviour
{
    [SerializeField] private int cooldown = 2;
    [SerializeField] private GameObject panel = null;
    [SerializeField] private Text text = null;
    private int turnsUntilAvailable = 0;


    private void Start()
    {
        panel.SetActive(false);
    }


    public void GoOnCooldown()
    {
        turnsUntilAvailable = cooldown;
        panel.SetActive(true);
    }


    public void OnTurnStart()
    {
        if (--turnsUntilAvailable == 0)
        {
            panel.SetActive(false);
        }
        else
        {
            text.text = turnsUntilAvailable.ToString();
        }
    }
}
