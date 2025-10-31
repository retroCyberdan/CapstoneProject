using UnityEngine;


[CreateAssetMenu(fileName = "New Basic Enemy", menuName = "Enemies/Basic Enemy")]
public class SO_BasicEnemy : SO_Enemy
{
    // può avere parametri aggiuntivi specifici
    public override void Initialize(EnemyController controller)
    {
        base.Initialize(controller);
        // logica specifica per il nemico base
    }
}