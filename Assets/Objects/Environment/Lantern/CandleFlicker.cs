using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleFlicker : MonoBehaviour, IPausable
{
    [SerializeField] Light pointLight = null;
    [SerializeField] float m = 0.1f;
    [SerializeField] float r = 0.95f;
    [SerializeField] float baseIntensity = 2.0f;

    private bool paused = false;
    private float t;

    // Start is called before the first frame update
    void Start()
    {
        t = Random.Range(0.0f, Mathf.PI * 2.0f);
        pointLight.transform.localPosition = Vector3.zero;
        pointLight.intensity = baseIntensity;
    }

    // Update is called once per frame
    void Update()
    {
        if (paused) return;

        //float rf = r * Time.deltaTime * 60.0f;
        //float mf = m * Time.deltaTime * 60.0f;

        //pointLight.transform.localPosition *= rf;
        //Vector3 dpos = new Vector3(
        //    Random.Range(-mf, mf),
        //    Random.Range(-mf, mf),
        //    Random.Range(-mf, mf));
        //pointLight.transform.localPosition += dpos;

        //float di = pointLight.intensity - baseIntensity;
        //pointLight.intensity = baseIntensity + rf * di + Random.Range(-mf, mf);

        t += Time.deltaTime;

        pointLight.intensity = baseIntensity + m * Mathf.Sin(r * t);
    }


    public void Pause()
    {
        paused = true;
    }


    public void Unpause()
    {
        paused = false;
    }
}
