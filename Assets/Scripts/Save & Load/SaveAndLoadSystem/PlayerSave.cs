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

    public List<string> collectedItemIDs = new List<string>(); // <- lista degli ID degli oggetti raccolti

    public bool introVista = false; // <- flag per tracciare se l'intro è già stata vista

    public PlayerSave() { }

    public PlayerSave(float[] position, float[] rotation, float currentHealth, float maxHealth, float currentStress, float maxStress, List<string> collectedItemIDs, bool introVista)
    {
        this.position = position;
        this.rotation = rotation;
        this.currentHealth = currentHealth;
        this.maxHealth = maxHealth;
        this.currentStress = currentStress;
        this.maxStress = maxStress;
        this.collectedItemIDs = collectedItemIDs ?? new List<string>();
        this.introVista = introVista;
    }
}