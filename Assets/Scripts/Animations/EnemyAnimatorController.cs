using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    private Animator _animator;
    private EnemyAI _enemyAI;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _enemyAI = GetComponent<EnemyAI>();
    }

    // Update is called once per frame
    void Update()
    {
        _animator.SetFloat("horizontal", _enemyAI.MoveDir.x);
        _animator.SetFloat("vertical", _enemyAI.MoveDir.z);
    }
}
