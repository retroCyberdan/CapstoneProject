using UnityEngine;


[CreateAssetMenu(fileName = "New Basic Enemy", menuName = "Enemies/Basic Enemy")]
public class SO_BasicEnemy : SO_Enemy
{
    // Può avere parametri aggiuntivi specifici
    public override void Initialize(EnemyController controller)
    {
        base.Initialize(controller);
        // Logica specifica per il nemico base
    }
}