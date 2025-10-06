using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Slider healthSlider;
    public Slider easeHealthSlider;
    public float maxHealthValue = 100f;
    public float healthValue;

    private float _lerpSpeed = .05f;

    // Start is called before the first frame update
    void Start()
    {
        healthValue = maxHealthValue;
    }

    // Update is called once per frame
    void Update()
    {
        if (healthSlider.value != healthValue) healthSlider.value = healthValue;

        //if (Input.GetKeyDown(KeyCode.H)) TakeDamage(10f);
        //if (Input.GetKeyDown(KeyCode.J)) healthValue += 10f;

        if (healthSlider.value > easeHealthSlider.value) easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, healthValue, _lerpSpeed); // <- guarigione
        
        else if (easeHealthSlider.value != healthValue) easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, healthValue, _lerpSpeed); // <- danno
    }

    void TakeDamage(float damage)
    {
        healthValue -= damage;
    }

}