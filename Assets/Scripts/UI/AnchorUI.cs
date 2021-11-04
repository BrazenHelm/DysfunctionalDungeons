using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorUI : MonoBehaviour
{
    public Transform anchor;
    private RectTransform rt;
    private Camera mc;


    private void Start()
    {
        mc = Camera.main;
        rt = GetComponent<RectTransform>();
    }


    private void LateUpdate()
    {
        if (anchor == null) return;
        rt.position = RectTransformUtility.WorldToScreenPoint(mc, anchor.position);
    }
}
