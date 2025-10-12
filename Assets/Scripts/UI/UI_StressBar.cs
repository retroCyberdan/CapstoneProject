using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StressBar : MonoBehaviour
{
    public Slider stressSlider;
    public Slider easeStressSlider;
    public float maxStressValue = 100f;
    public float stressValue;

    private float _lerpSpeed = .05f;

    // Start is called before the first frame update
    void Start()
    {
        stressValue = 0f; // <- la barra parte da 0
    }

    // Update is called once per frame
    void Update()
    {
        if (stressSlider.value != stressValue) stressSlider.value = stressValue;

        if (stressSlider.value > easeStressSlider.value) easeStressSlider.value = Mathf.Lerp(easeStressSlider.value, stressValue, _lerpSpeed); // <- aumento stress

        else if (easeStressSlider.value != stressValue) easeStressSlider.value = Mathf.Lerp(easeStressSlider.value, stressValue, _lerpSpeed); // <- riduzione stress
    }
}