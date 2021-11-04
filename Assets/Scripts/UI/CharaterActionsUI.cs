using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaterActionsUI : MonoBehaviour
{
    private GameObject popup;


    private void Awake()
    {
        popup = transform.GetChild(0).gameObject;
    }


    void Start()
    {
        popup.SetActive(false);
    }


    public void Show()
    {
        popup.SetActive(true);
    }


    public void Hide()
    {
        popup.SetActive(false);
    }


    public void OnButtonPress(int index)
    {
        FindObjectOfType<GameController>().SetMode(index);
    }
}

