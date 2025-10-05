using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _minWaitTime = 2f;
    [SerializeField] private float _maxWaitTime = 5f;
    [SerializeField] private float _minMoveTime = 2f;
    [SerializeField] private float _maxMoveTime = 4f;
    [SerializeField] private float _rotationSpeed = 10f;

    private CharacterController _characterController;
    private EnemyVisionAI _enemyVision;
    private Vector3 _randomDirection;
    private Vector3 _moveDir;
    private float _timer;
    private float _actionTime;
    private bool _isMoving;

    public Vector3 MoveDir => _moveDir;

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _enemyVision = GetComponent<EnemyVisionAI>();

        ChangeState();
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        _moveDir = Vector3.zero;

        if (_enemyVision != null && _enemyVision.IsChasingPlayer)
        {
            // insegue il player
            Vector3 dir = _enemyVision.GetDirectionToPlayer();
            _moveDir = new Vector3(dir.x, 0, dir.z);
        }
        else
        {
            // movimento randomico
            _timer += Time.deltaTime;

            if (_timer >= _actionTime) ChangeState();

            if (_isMoving) _moveDir = _randomDirection;
        }

        // ruota il nemico nella direzione del movimento
        if (_moveDir.x != 0 || _moveDir.z != 0)
        {
            Vector3 lookDirection = new Vector3(_moveDir.x, 0, _moveDir.z);
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }

        // applica gravità e movimento
        _moveDir.y = -9.81f;
        _characterController.Move(_moveDir * _moveSpeed * Time.deltaTime);
    }

    void ChangeState()
    {
        _timer = 0f;
        _isMoving = !_isMoving;

        if (_isMoving)
        {
            // genera direzione random
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            _randomDirection = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)).normalized;
            _actionTime = Random.Range(_minMoveTime, _maxMoveTime);
        }
        else
        {
            _actionTime = Random.Range(_minWaitTime, _maxWaitTime);
        }
    }
}