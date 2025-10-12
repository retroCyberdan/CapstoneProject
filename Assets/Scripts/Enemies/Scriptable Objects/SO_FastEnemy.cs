using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Fast Enemy", menuName = "Enemies/Fast Enemy")]
public class SO_FastEnemy : SO_Enemy
{
    [Header("Fast Enemy Specific")]
    public float speedMultiplier = 1.5f;

    public override void Initialize(EnemyController controller)
    {
        base.Initialize(controller);
        // Logica specifica per il nemico veloce
    }
}