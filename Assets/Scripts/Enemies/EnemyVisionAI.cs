using UnityEngine;

public class EnemyVisionAI : MonoBehaviour
{
    [Header("Vision Settings")]
    [SerializeField] private float _visionRadius = 10f;
    [SerializeField] private int _fovSegments = 50;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private LayerMask _obstacleLayer;

    [Header("Debug")]
    [SerializeField] private bool _drawFOV = true;

    private LineRenderer _lineRenderer;
    private Transform _player;
    private bool _chasingPlayer;

    public bool IsChasingPlayer => _chasingPlayer;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (_drawFOV) FovLineRendererSetup();
    }

    void Update()
    {
        CheckForPlayer();
        if (_drawFOV) DrawFOV();
    }

    void FovLineRendererSetup()
    {
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.positionCount = _fovSegments + 1;
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.loop = true;

        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }

    void CheckForPlayer()
    {
        if (_player == null)
        {
            _chasingPlayer = false;
            return;
        }

        float distToPlayer = Vector3.Distance(transform.position, _player.position);

        if (distToPlayer <= _visionRadius)
        {
            Vector3 dirToPlayer = (_player.position - transform.position).normalized;

            if (Physics.SphereCast(transform.position, 0.5f, dirToPlayer, out RaycastHit hit, _visionRadius, _playerLayer | _obstacleLayer)) // <- sphereCast per controllare se c'è line of sight
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

    void DrawFOV()
    {
        if (_lineRenderer == null) return;

        // disegna un FOV circolare
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

    public Vector3 GetDirectionToPlayer()
    {
        if (_player == null) return Vector3.zero;
        return (_player.position - transform.position).normalized;
    }

    public void SetVisionRadius(float radius) // <- metodo per impostare il raggio di visione da EnemyController
    {
        _visionRadius = Mathf.Max(0f, radius);
    }

    public void SetFovSegments(int segments) // <- metodo per impostare i segmenti del FOV da EnemyController
    {
        _fovSegments = Mathf.Max(3, segments);
        if (_lineRenderer != null)
        {
            _lineRenderer.positionCount = _fovSegments + 1;
        }
    }

    public float GetVisionRadius() => _visionRadius; // <- permette di ottenere il raggio di visione attuale
}