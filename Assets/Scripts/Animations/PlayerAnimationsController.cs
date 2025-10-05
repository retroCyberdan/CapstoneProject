using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationsController : MonoBehaviour
{
    private Animator _animator;
    private PlayerController _playerController;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        _animator.SetFloat("horizontal", _playerController.Horizontal);
        _animator.SetFloat("vertical", _playerController.Vertical);
        _animator.SetFloat("speed", _playerController.CurrentSpeed);
    }
}