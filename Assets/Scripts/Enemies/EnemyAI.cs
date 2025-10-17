using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _minWaitTime = 2f;
    [SerializeField] private float _maxWaitTime = 5f;
    [SerializeField] private float _minMoveTime = 2f;
    [SerializeField] private float _maxMoveTime = 4f;
    [SerializeField] private float _rotationSpeed = 10f;

    [Header("Vision Settings")]
    [SerializeField] private float _visionRadius = 10f;
    [SerializeField] private int _fovSegments = 50;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private LayerMask _obstacleLayer;

    [Header("Damage Settings")]
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _damageCooldown = 1f;

    private CharacterController _characterController;
    private LineRenderer _lineRenderer;
    private Transform _player;
    private HealthSystem _playerHealth;
    private Vector3 _randomDirection;
    private Vector3 moveDir;
    private float _timer;
    private float _actionTime;
    private float _lastDamageTime;
    private bool _isMoving;
    private bool _chasingPlayer;

    public Vector3 MoveDir => moveDir;

    void Start()
    {
        _characterController = GetComponent<CharacterController>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            _player = playerObj.transform;
            _playerHealth = playerObj.GetComponent<HealthSystem>();
        }

        FovLineRendererSetup();
        ChangeState();
    }

    void Update()
    {
        CheckForPlayer();
        HandleMovement();
        DrawFOV();
    }

    void FovLineRendererSetup()
    {
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.positionCount = _fovSegments + 1;
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.loop = true;

        // assegna un materiale di default per evitare il colore viola
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }

    void CheckForPlayer()
    {
        if (_player == null) return;

        float distToPlayer = Vector3.Distance(transform.position, _player.position);

        if (distToPlayer <= _visionRadius)
        {
            Vector3 dirToPlayer = (_player.position - transform.position).normalized;

            // spherecast per controllare se c'è LOS
            if (Physics.SphereCast(transform.position, 0.5f, dirToPlayer, out RaycastHit hit, _visionRadius, _playerLayer | _obstacleLayer))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    _chasingPlayer = true;
                    return;
                }
            }
        }

        _chasingPlayer = false;
    }

    void HandleMovement()
    {
        moveDir = Vector3.zero;

        if (_chasingPlayer && _player != null)
        {
            // insegue il player
            Vector3 dir = (_player.position - transform.position).normalized;
            moveDir = new Vector3(dir.x, 0, dir.z);
        }
        else
        {
            _timer += Time.deltaTime;

            if (_timer >= _actionTime) ChangeState();

            if (_isMoving) moveDir = _randomDirection;
        }

        // Ruota il nemico nella direzione del movimento
        if (moveDir.x != 0 || moveDir.z != 0)
        {
            Vector3 lookDirection = new Vector3(moveDir.x, 0, moveDir.z);
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }

        moveDir.y = -9.81f;
        _characterController.Move(moveDir * _moveSpeed * Time.deltaTime);
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
        else _actionTime = Random.Range(_minWaitTime, _maxWaitTime);
    }

    void DrawFOV()
    {
        for (int i = 0; i <= _fovSegments; i++)
        {
            float angle = (i / (float)_fovSegments) * 2 * Mathf.PI;
            float x = Mathf.Cos(angle) * _visionRadius;
            float z = Mathf.Sin(angle) * _visionRadius;

            Vector3 pos = transform.position + new Vector3(x, 0.1f, z);
            _lineRenderer.SetPosition(i, pos);
        }

        // colore diverso se sta inseguendo
        Color fovColor = _chasingPlayer ? Color.red : Color.yellow;
        _lineRenderer.startColor = fovColor;
        _lineRenderer.endColor = fovColor;
    }

    // Gestione collisione con CharacterController
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Player"))
        {
            DealDamageToPlayer();
        }
    }

    // Gestione collisione con Rigidbody
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DealDamageToPlayer();
        }
    }

    // Gestione trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DealDamageToPlayer();
        }
    }

    private void DealDamageToPlayer()
    {
        if (Time.time - _lastDamageTime >= _damageCooldown)
        {
            if (_playerHealth != null && _playerHealth.IsAlive())
            {
                _playerHealth.TakeDamage(_damage);
                _lastDamageTime = Time.time;
                Debug.Log($"{gameObject.name} ha inflitto {_damage} danni al player!");
            }
        }
    }

    // Metodi pubblici per modificare il danno
    public void SetDamage(float newDamage) => _damage = Mathf.Max(0, newDamage);
    public float GetDamage() => _damage;
}