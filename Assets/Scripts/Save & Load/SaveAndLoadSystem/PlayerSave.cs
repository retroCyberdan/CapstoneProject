using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // <- la rendo serializzabile
public class PlayerSave // <-  rimuovo MonoBehaviour poichè deve essere scollegata da qualsiasi componente
{
    public float[] position = new float[3];
    public float[] rotation = new float[4];
    public float currentHealth;
    public float maxHealth;
    public float currentStress;
    public float maxStress;

    public PlayerSave() { } // <- costruttore vuoto

    public PlayerSave(float[] position, float[] rotation, float currentHealth, float maxHealth, float currentStress, float maxStress) // <- override costruttore con parametri
    {
        this.position = position;
        this.rotation = rotation;
        this.currentHealth = currentHealth;
        this.maxHealth = maxHealth;
        this.currentStress = currentStress;
        this.maxStress = maxStress;
    }
}