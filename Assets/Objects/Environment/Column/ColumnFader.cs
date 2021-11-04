using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnFader : MonoBehaviour
{
    [SerializeField] int nLayers = 5;
    [SerializeField] GameObject layerPrefab = null;
    [SerializeField] Material material = null;

    private static Material[] mats;
    private static bool isMatsInitialised = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!isMatsInitialised)
        {
            mats = new Material[nLayers];
            mats[0] = material;

            float alphaStep = 1.0f / nLayers;

            for (int i = 1; i < nLayers; i++)
            {
                mats[i] = new Material(material);
                mats[i].color = new Color(
                    material.color.r,
                    material.color.g,
                    material.color.b,
                    1.0f - i * alphaStep);
            }

            isMatsInitialised = true;
        }

        for (int i = 0; i < nLayers; i++)
        {
            GameObject layer = Instantiate(layerPrefab, transform);
            layer.transform.Translate(0, -(i + 0.5f) * transform.localScale.y, 0);
            
            for (int j = 0; j < layer.transform.childCount; j++)
            {
                layer.transform.GetChild(j).GetComponent<MeshRenderer>().material = mats[i];
            }
        }
    }
}
