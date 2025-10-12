using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Scout Enemy", menuName = "Enemies/Scout Enemy")]
public class SO_ScoutEnemy : SO_Enemy
{
    [Header("Scout Enemy Specific")]
    public float visionMultiplier = 1.5f;

    public override void Initialize(EnemyController controller)
    {
        base.Initialize(controller);
        // Logica specifica per il nemico scout
    }
}