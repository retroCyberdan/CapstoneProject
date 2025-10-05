using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public GameObject on;
    public GameObject off;

    [Header("Audio Settings")]
    public AudioClip flashlightToggleSound;
    [Range(0f, 1f)] public float flashlightVolume = 0.7f;

    private bool _isOn;

    void Start()
    {
        on.SetActive(false);
        off.SetActive(true);
        _isOn = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) UseFlashlight();
    }

    private void UseFlashlight()
    {
        if (_isOn)
        {
            on.SetActive(false);
            off.SetActive(true);
        }
        else
        {
            on.SetActive(true);
            off.SetActive(false);
        }

        _isOn = !_isOn;

        if (AudioManager.Instance != null && flashlightToggleSound != null)
        {
            AudioManager.Instance.PlayOneShot(flashlightToggleSound, transform.position, flashlightVolume);
        }
    }
}