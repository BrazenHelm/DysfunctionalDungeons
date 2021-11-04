using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBoxLayout : MonoBehaviour
{
    [SerializeField] private Image portrait = null;
    [SerializeField] private Text nameText = null;
    [SerializeField] private Text speechText = null;


    public void SetCharacter(Sprite picture, string name)
    {
        portrait.sprite = picture;
        nameText.text = name;
    }


    public void SetSpeech(string speech)
    {
        speechText.text = speech;
    }
}
