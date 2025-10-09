using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    private Animator _animator;
    private EnemyController _enemyController;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _enemyController = GetComponent<EnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
        _animator.SetFloat("horizontal", _enemyController.MoveDir.x);
        _animator.SetFloat("vertical", _enemyController.MoveDir.z);
    }
}
