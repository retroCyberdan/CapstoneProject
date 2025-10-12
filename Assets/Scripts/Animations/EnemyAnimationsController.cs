using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationsController : MonoBehaviour
{
    private Animator _animator;
    private EnemyController _enemyController;
    private bool _isAttacking;

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
        _animator.SetBool("isAttacking", _isAttacking);
    }

    // Metodo chiamato quando il nemico triggera il player
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Player"))
        {
            TriggerAttack();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TriggerAttack();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerAttack();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isAttacking = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _isAttacking = false;
        }
    }

    private void TriggerAttack()
    {
        _isAttacking = true;
    }
}