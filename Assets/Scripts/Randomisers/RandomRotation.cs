using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    void Start()
    {
        Debug.Log("performing random rotation on " + gameObject.name);
        transform.Rotate(Vector3.up, Random.Range(0.0f, 360.0f));
    }  
}
