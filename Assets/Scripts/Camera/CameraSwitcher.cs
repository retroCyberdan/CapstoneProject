using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    public Transform playerTransform;
    public CinemachineVirtualCamera activeCamera;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) activeCamera.Priority = 20; // Aumenta la priorità per attivare questa telecamera
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) activeCamera.Priority = 0; // Ripristina la priorità per disattivare questa telecamera
    }
}
