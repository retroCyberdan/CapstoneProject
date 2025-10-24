using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSave
{
    public float[] position = new float[3];
    public float[] rotation = new float[4];
    public float currentHealth;
    public float maxHealth;
    public float currentStress;
    public float maxStress;

    // Lista degli ID degli oggetti raccolti
    public List<string> collectedItemIDs = new List<string>();

    public PlayerSave() { }

    public PlayerSave(float[] position, float[] rotation, float currentHealth, float maxHealth,
                      float currentStress, float maxStress, List<string> collectedItemIDs)
    {
        this.position = position;
        this.rotation = rotation;
        this.currentHealth = currentHealth;
        this.maxHealth = maxHealth;
        this.currentStress = currentStress;
        this.maxStress = maxStress;
        this.collectedItemIDs = collectedItemIDs ?? new List<string>();
    }
}