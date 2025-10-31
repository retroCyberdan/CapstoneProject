using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Tank Enemy", menuName = "Enemies/Tank Enemy")]
public class SO_TankEnemy : SO_Enemy
{
    [Header("Tank Enemy Specific")]
    public float damageMultiplier = 2f;
    public float moveSpeedReduction = 0.7f;

    public override void Initialize(EnemyController controller)
    {
        base.Initialize(controller);
        // logica specifica per il nemico tank
    }
}