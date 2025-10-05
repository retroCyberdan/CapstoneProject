using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 2.5f;
    public float sprintSpeed = 5f;
    private float _currentSpeed;

    [Header("Gravity Settings")]
    public float gravity = -9.81f;
    private float _verticalVelocity;

    [Header("Audio Settings")]
    public float footstepInterval = 0.5f;
    public float sprintFootstepMultiplier = 1.8f;
    public float groundCheckDistance = 0.1f; // <- distanza per il raycast che rileva il terreno
    private float _lastFootstepTime;
    private string _currentSurfaceType = "Ground"; // <- tipo di superficie corrente

    private float _horizontal;
    private float _vertical;
    private CharacterController _characterController;
    private bool _wasRunning = false; // <- traccia lo stato di corsa

    public float Horizontal => _horizontal;
    public float Vertical => _vertical;
    public float CurrentSpeed => _currentSpeed;

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _currentSpeed = walkSpeed;
    }

    void Update()
    {
        HandleInput();
        Movement();
        DetectSurface(); // <- rileva il tipo di superficie sotto il player
        HandleFootsteps();
        HandleBreathing(); // <- gestisce i suoni di affanno
    }

    private void HandleInput()
    {
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.LeftShift)) _currentSpeed = sprintSpeed;
        else _currentSpeed = walkSpeed;
    }

    private void Movement()
    {
        // movimento orizzontale
        Vector3 direction = transform.TransformDirection(new Vector3(_horizontal, 0, _vertical)).normalized;
        Vector3 horizontalMove = direction * _currentSpeed * Time.deltaTime;

        // applica la gravità
        if (_characterController.isGrounded && _verticalVelocity < 0) _verticalVelocity = -2f; // piccola forza per mantenere il player a terra

        else _verticalVelocity += gravity * Time.deltaTime;

        // movimento verticale (gravità)
        Vector3 verticalMove = new Vector3(0, _verticalVelocity, 0) * Time.deltaTime;

        // combina movimento orizzontale e verticale
        _characterController.Move(horizontalMove + verticalMove);
    }

    private void HandleFootsteps()
    {
        bool isMoving = Mathf.Abs(_horizontal) > 0.1f || Mathf.Abs(_vertical) > 0.1f;
        bool isGrounded = _characterController.isGrounded;
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        if (isMoving && isGrounded)
        {
            float currentFootstepInterval = GetCurrentFootstepInterval();

            if (Time.time - _lastFootstepTime > currentFootstepInterval)
            {
                PlayFootstepSound(isSprinting); // <- passa lo stato di sprint
                _lastFootstepTime = Time.time;
            }
        }
    }

    private void HandleBreathing() // <- gestisce l'attivazione/disattivazione dei suoni di affanno
    {
        bool isMoving = Mathf.Abs(_horizontal) > 0.1f || Mathf.Abs(_vertical) > 0.1f;
        bool isGrounded = _characterController.isGrounded;
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        bool isRunning = isMoving && isGrounded && isSprinting;

        if (isRunning && !_wasRunning)
        {
            // Ha appena iniziato a correre
            if (AudioManager.Instance != null) AudioManager.Instance.StartRunningBreathing(transform.position);
        }
        else if (!isRunning && _wasRunning)
        {
            // Ha appena smesso di correre
            if (AudioManager.Instance != null) AudioManager.Instance.StopRunningBreathing();
        }

        _wasRunning = isRunning;
    }

    private void DetectSurface() // <- rileva il tipo di superficie sotto il player tramite raycast
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position;

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, groundCheckDistance + 1f))
        {
            // controlla il layer dell'oggetto colpito (con spazio "Wood Ground")
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wood Ground"))
            {
                _currentSurfaceType = "WoodGround";
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                _currentSurfaceType = "Ground";
            }
            else
            {
                _currentSurfaceType = "Ground"; // default
            }
        }
    }

    private float GetCurrentFootstepInterval()
    {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        return isSprinting ? footstepInterval / sprintFootstepMultiplier : footstepInterval;
    }

    private void PlayFootstepSound(bool isRunning) // <- ora accetta il parametro isRunning
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayFootstep(transform.position, isRunning, _currentSurfaceType);
    }
}