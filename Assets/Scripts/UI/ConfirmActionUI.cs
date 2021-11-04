using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmActionUI : MonoBehaviour
{
    GameController gc = null;

    private void Awake()
    {
        gc = FindObjectOfType<GameController>();
    }

}
