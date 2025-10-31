using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObjectsTrigger : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private Canvas popUpCanvas;

    [Header("Movement Settings")]
    [SerializeField] private float pushSpeed = 1.5f;
    [SerializeField] private float smoothTime = 0.1f;

    private bool _isPlayerInRange = false;
    private bool _isBeingPushed = false;
    private Transform _playerTransform;
    private PlayerController _playerController;
    private Rigidbody _objectRigidbody;
    private Vector3 _velocity = Vector3.zero;

    private void Awake()
    {
        if (popUpCanvas != null) popUpCanvas.gameObject.SetActive(false);

        _objectRigidbody = GetComponent<Rigidbody>();

        // configura il Rigidbody se presente
        if (_objectRigidbody != null)
        {
            _objectRigidbody.isKinematic = false;
            _objectRigidbody.useGravity = true;
            _objectRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    private void Update()
    {
        // Inizia a spingere quando premi E
        if (_isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !_isBeingPushed) StartPushing();

        // Smette di spingere quando rilasci E
        if (_isBeingPushed && Input.GetKeyUp(KeyCode.E)) StopPushing();

        // Gestisce il movimento mentre stai spingendo
        if (_isBeingPushed && _playerController != null) HandlePushing();
    }

    private void StartPushing()
    {
        _isBeingPushed = true;

        if (_playerController != null) _playerController.canSprint = false; // <- disabilita lo sprint del player mentre spinge
    }

    private void StopPushing()
    {
        _isBeingPushed = false;

        if (_playerController != null) _playerController.canSprint = true; // <- riabilita lo sprint del player

        if (_objectRigidbody != null) _objectRigidbody.velocity = Vector3.zero; // <- ferma completamente l'oggetto
    }

    private void HandlePushing()
    {
        // ottiene l'input del player
        float horizontal = _playerController.Horizontal;
        float vertical = _playerController.Vertical;

        // se il player non si sta muovendo, non fare nulla
        if (Mathf.Abs(horizontal) < 0.1f && Mathf.Abs(vertical) < 0.1f)
        {
            if (_objectRigidbody != null) _objectRigidbody.velocity = new Vector3(0, _objectRigidbody.velocity.y, 0);

            return;
        }

        // calcola la direzione di movimento dell'oggetto in base all'input del player
        Vector3 pushDirection = _playerTransform.TransformDirection(new Vector3(horizontal, 0, vertical)).normalized;
        pushDirection.y = 0; // <- si assicura che l'oggetto si muova solo orizzontalmente

        // muove l'oggetto
        Vector3 targetVelocity = pushDirection * pushSpeed;

        if (_objectRigidbody != null)
        {
            // mantiene la velocità Y (gravità) e applica solo il movimento orizzontale
            targetVelocity.y = _objectRigidbody.velocity.y;
            _objectRigidbody.velocity = Vector3.SmoothDamp(_objectRigidbody.velocity, targetVelocity, ref _velocity, smoothTime);
        }
        else
        {
            // se non c'è Rigidbody, usa Transform.Translate
            transform.Translate(pushDirection * pushSpeed * Time.deltaTime, Space.World);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInRange = true;
            _playerTransform = other.transform;
            _playerController = other.GetComponent<PlayerController>();

            if (popUpCanvas != null) popUpCanvas.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInRange = false;

            if (_isBeingPushed) StopPushing(); // <- se stava spingendo, ferma tutto

            _playerTransform = null;
            _playerController = null;

            if (popUpCanvas != null) popUpCanvas.gameObject.SetActive(false);
        }
    }
}