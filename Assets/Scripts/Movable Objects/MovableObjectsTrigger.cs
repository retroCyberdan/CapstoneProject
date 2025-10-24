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

    private bool isPlayerInRange = false;
    private bool isBeingPushed = false;
    private Transform playerTransform;
    private PlayerController playerController;
    private Rigidbody objectRigidbody;
    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        if (popUpCanvas != null)
            popUpCanvas.gameObject.SetActive(false);

        objectRigidbody = GetComponent<Rigidbody>();

        // Configura il Rigidbody se presente
        if (objectRigidbody != null)
        {
            objectRigidbody.isKinematic = false;
            objectRigidbody.useGravity = true;
            objectRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    private void Update()
    {
        // Inizia a spingere quando premi E
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !isBeingPushed)
        {
            StartPushing();
        }

        // Smetti di spingere quando rilasci E
        if (isBeingPushed && Input.GetKeyUp(KeyCode.E))
        {
            StopPushing();
        }

        // Gestisci il movimento mentre stai spingendo
        if (isBeingPushed && playerController != null)
        {
            HandlePushing();
        }
    }

    private void StartPushing()
    {
        isBeingPushed = true;

        // Disabilita lo sprint del player mentre spinge
        if (playerController != null)
        {
            playerController.canSprint = false;
        }
    }

    private void StopPushing()
    {
        isBeingPushed = false;

        // Riabilita lo sprint del player
        if (playerController != null)
        {
            playerController.canSprint = true;
        }

        // Ferma completamente l'oggetto
        if (objectRigidbody != null)
        {
            objectRigidbody.velocity = Vector3.zero;
        }
    }

    private void HandlePushing()
    {
        // Ottieni l'input del player
        float horizontal = playerController.Horizontal;
        float vertical = playerController.Vertical;

        // Se il player non si sta muovendo, non fare nulla
        if (Mathf.Abs(horizontal) < 0.1f && Mathf.Abs(vertical) < 0.1f)
        {
            if (objectRigidbody != null)
            {
                objectRigidbody.velocity = new Vector3(0, objectRigidbody.velocity.y, 0);
            }
            return;
        }

        // Calcola la direzione di movimento dell'oggetto in base all'input del player
        Vector3 pushDirection = playerTransform.TransformDirection(new Vector3(horizontal, 0, vertical)).normalized;
        pushDirection.y = 0; // Assicurati che l'oggetto si muova solo orizzontalmente

        // Muovi l'oggetto
        Vector3 targetVelocity = pushDirection * pushSpeed;

        if (objectRigidbody != null)
        {
            // Mantieni la velocità Y (gravità) e applica solo il movimento orizzontale
            targetVelocity.y = objectRigidbody.velocity.y;
            objectRigidbody.velocity = Vector3.SmoothDamp(
                objectRigidbody.velocity,
                targetVelocity,
                ref velocity,
                smoothTime
            );
        }
        else
        {
            // Se non c'è Rigidbody, usa Transform.Translate
            transform.Translate(pushDirection * pushSpeed * Time.deltaTime, Space.World);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerTransform = other.transform;
            playerController = other.GetComponent<PlayerController>();

            if (popUpCanvas != null)
                popUpCanvas.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;

            // Se stava spingendo, ferma tutto
            if (isBeingPushed)
            {
                StopPushing();
            }

            playerTransform = null;
            playerController = null;

            if (popUpCanvas != null)
                popUpCanvas.gameObject.SetActive(false);
        }
    }
}