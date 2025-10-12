using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Data")]
    [SerializeField] private SO_Enemy _enemyData;

    // Parametri di movimento (caricati dallo ScriptableObject)
    private float _moveSpeed;
    private float _minWaitTime;
    private float _maxWaitTime;
    private float _minMoveTime;
    private float _maxMoveTime;
    private float _rotationSpeed;

    // Parametri di danno (caricati dallo ScriptableObject)
    private float _damage;
    private float _damageCooldown;

    private CharacterController _characterController;
    private EnemyVisionAI _enemyVision;
    private HealthSystem _playerHealth;
    private Vector3 _randomDirection;
    private Vector3 _moveDir;
    private float _timer;
    private float _actionTime;
    private float _lastDamageTime;
    private bool _isMoving;

    public Vector3 MoveDir => _moveDir;
    public SO_Enemy EnemyData => _enemyData;

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _enemyVision = GetComponent<EnemyVisionAI>();

        // Trova e salva il riferimento al HealthSystem del player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            _playerHealth = playerObj.GetComponent<HealthSystem>();
        }

        // Carica i parametri dallo ScriptableObject
        LoadEnemyData();

        ChangeState();
    }

    /// <summary>
    /// Carica tutti i parametri dallo ScriptableObject
    /// </summary>
    private void LoadEnemyData()
    {
        if (_enemyData == null)
        {
            Debug.LogError($"{gameObject.name}: Nessun SO_Enemy assegnato!");
            return;
        }

        _moveSpeed = _enemyData.moveSpeed;
        _minWaitTime = _enemyData.minWaitTime;
        _maxWaitTime = _enemyData.maxWaitTime;
        _minMoveTime = _enemyData.minMoveTime;
        _maxMoveTime = _enemyData.maxMoveTime;
        _rotationSpeed = _enemyData.rotationSpeed;
        _damage = _enemyData.damage;
        _damageCooldown = _enemyData.damageCooldown;

        // Configura EnemyVisionAI se presente
        if (_enemyVision != null)
        {
            _enemyVision.SetVisionRadius(_enemyData.visionRadius);
            _enemyVision.SetFovSegments(_enemyData.fovSegments);
        }

        // Chiama il metodo di inizializzazione custom del nemico
        _enemyData.Initialize(this);
    }

    /// <summary>
    /// Permette di cambiare i dati del nemico a runtime (utile per il pooling)
    /// </summary>
    public void SetEnemyData(SO_Enemy newEnemyData)
    {
        _enemyData = newEnemyData;
        LoadEnemyData();
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

    // Metodi pubblici per modificare il danno a runtime
    public void SetDamage(float newDamage) => _damage = Mathf.Max(0, newDamage);
    public float GetDamage() => _damage;

    // Metodi pubblici per modificare altri parametri a runtime
    public void SetMoveSpeed(float newSpeed) => _moveSpeed = Mathf.Max(0, newSpeed);
    public float GetMoveSpeed() => _moveSpeed;
}