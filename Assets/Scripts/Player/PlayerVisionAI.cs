using System.Collections.Generic;
using UnityEngine;

public class PlayerVisionAI : MonoBehaviour
{
    [Header("Vision Settings")]
    [SerializeField] private float _visionRadius = 15f;
    [SerializeField] private int _fovSegments = 50;
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private LayerMask _obstacleLayer;

    [Header("Debug")]
    [SerializeField] private bool _drawFOV = true;

    private LineRenderer _lineRenderer;
    private List<GameObject> _enemiesInSight = new List<GameObject>();

    public int EnemiesInSightCount => _enemiesInSight.Count;

    void Start()
    {
        if (_drawFOV) FovLineRendererSetup();
    }

    void Update()
    {
        CheckForEnemies();
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

        // Assegna un materiale di default
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.startColor = Color.cyan;
        _lineRenderer.endColor = Color.cyan;
    }

    void CheckForEnemies()
    {
        _enemiesInSight.Clear();

        // Trova tutti i nemici con tag "Enemy" e "Villain" attivi
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] villains = GameObject.FindGameObjectsWithTag("Boss");

        CheckEnemyArray(enemies);
        CheckEnemyArray(villains);
    }

    void CheckEnemyArray(GameObject[] enemies)
    {
        foreach (var enemy in enemies)
        {
            if (!enemy.activeInHierarchy) continue;

            float distToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            if (distToEnemy <= _visionRadius)
            {
                Vector3 dirToEnemy = (enemy.transform.position - transform.position).normalized;

                // SphereCast per controllare se c'è line of sight
                if (Physics.SphereCast(transform.position, 0.5f, dirToEnemy, out RaycastHit hit, _visionRadius, _enemyLayer | _obstacleLayer))
                {
                    if (hit.transform.gameObject == enemy)
                    {
                        _enemiesInSight.Add(enemy);
                    }
                }
            }
        }
    }

    void DrawFOV()
    {
        if (_lineRenderer == null) return;

        // Disegna un FOV circolare
        for (int i = 0; i <= _fovSegments; i++)
        {
            float angle = (i / (float)_fovSegments) * 2 * Mathf.PI;
            float x = Mathf.Cos(angle) * _visionRadius;
            float z = Mathf.Sin(angle) * _visionRadius;

            Vector3 pos = transform.position + new Vector3(x, 0.1f, z);
            _lineRenderer.SetPosition(i, pos);
        }

        // Colore diverso se ci sono nemici in vista
        Color fovColor = _enemiesInSight.Count > 0 ? Color.red : Color.cyan;
        _lineRenderer.startColor = fovColor;
        _lineRenderer.endColor = fovColor;
    }

    public bool IsEnemyInSight(GameObject enemy)
    {
        return _enemiesInSight.Contains(enemy);
    }

    public List<GameObject> GetEnemiesInSight()
    {
        return new List<GameObject>(_enemiesInSight);
    }
}