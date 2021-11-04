using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnWallGenerator : MonoBehaviour
{
    [SerializeField] float minScale = 0.5f;
    [SerializeField] float maxScale = 1.0f;
    [SerializeField] float minHeight = 0.5f;
    [SerializeField] float maxHeight = 1.0f;
    [SerializeField] float desiredLength = 10.0f;
    [SerializeField] float lastColumnCutoff = 1.0f;
    [SerializeField] Vector3 direction = Vector3.right;

    [SerializeField] GameObject columnPrefab = null;

    private float lengthSoFar = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        while (lengthSoFar < desiredLength - lastColumnCutoff)
        {
            MakeColumn(Random.Range(minScale, maxScale));
        }

        if (lengthSoFar < desiredLength)
            MakeColumn(desiredLength - lengthSoFar);
    }


    private void MakeColumn(float size)
    {
        lengthSoFar += size;
        GameObject obj = Instantiate(columnPrefab, transform);
        obj.transform.Translate(direction * (lengthSoFar - size / 2.0f));
        obj.transform.localScale = new Vector3(
            size, Random.Range(minHeight, maxHeight), size);
    }


}
