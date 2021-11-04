using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTextureShift : MonoBehaviour
{
    [SerializeField] Material[] mats = null;
    [SerializeField] bool horizontal = false;
    [SerializeField] bool vertical = false;

    // Start is called before the first frame update
    void Start()
    {
        Material myMat = new Material(mats[Random.Range(0, mats.Length - 1)]);
        Vector2 offset = myMat.GetTextureOffset("_MainTex");

        if (horizontal)
        {
            offset.x += Random.Range(0f, 1f);
        }
        if (vertical)
        {
            offset.y += Random.Range(0f, 1f);
        }

        myMat.SetTextureOffset("_MainTex", offset);
        GetComponent<MeshRenderer>().material = myMat;
    }

    
}
