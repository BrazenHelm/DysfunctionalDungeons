using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DamageText : MonoBehaviour
{
    [SerializeField] private RectTransform textRectTransform = null;
    [SerializeField] private Text text = null;

    [SerializeField] private float timeBeforeFade = 1.0f;
    [SerializeField] private float timeToFade = 1.0f;
    [SerializeField] private float startingHeight = 50.0f;
    [SerializeField] private float distanceToRise = 50.0f;

    private float timeSinceSpawn = 0.0f;
    private float riseParam = 0.0f;
    private float fadeParam = 1.0f;


    private void Start()
    {
        
    }


    void Update()
    {
        timeSinceSpawn += Time.deltaTime;

        riseParam = timeSinceSpawn / (timeBeforeFade + timeToFade);
        textRectTransform.localPosition = Vector3.up * (startingHeight + distanceToRise * riseParam);

        if (timeSinceSpawn > timeBeforeFade)
        {
            fadeParam = (timeBeforeFade + timeToFade - timeSinceSpawn) / timeToFade;

            if (fadeParam < 0.0f)
            {
                Destroy(gameObject);
            }
            else
            {
                Color color = text.color;
                color.a = fadeParam;
                text.color = color;
            }
        }
    }


    public void SetValue(int value)
    {
        text.text = "-" + value.ToString() + " HP";
    }
}
