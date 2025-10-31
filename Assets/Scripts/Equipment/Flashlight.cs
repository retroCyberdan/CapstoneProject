using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [Header("Flashlight References")]
    public GameObject on;
    public GameObject off;

    [Header("Audio Settings")]
    public AudioClip flashlightToggleSound;
    [Range(0f, 1f)] public float flashlightVolume = 0.7f;

    [Header("Item Reference")]
    [Tooltip("Riferimento allo ScriptableObject della torcia")]
    public SO_Items flashlightItem;

    private bool _isOn;
    private bool _hasFlashlight;

    void Start()
    {
        CheckInventoryForFlashlight();
        UpdateFlashlightState();
    }

    void Update()
    {
        // Controlla solo se non abbiamo ancora la torcia
        if (!_hasFlashlight)
        {
            CheckInventoryForFlashlight();

            if (_hasFlashlight) UpdateFlashlightState();

            return;
        }

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

        if (AudioManager.Instance != null && flashlightToggleSound != null) AudioManager.Instance.PlayOneShot(flashlightToggleSound, transform.position, flashlightVolume);
    }

    private void CheckInventoryForFlashlight()
    {
        if (InventoryManager.Instance == null || flashlightItem == null)
        {
            _hasFlashlight = false;
            return;
        }

        _hasFlashlight = InventoryManager.Instance.HasItem(flashlightItem);
    }

    private void UpdateFlashlightState()
    {
        if (_hasFlashlight)
        {
            // mostra la torcia spenta
            on.SetActive(false);
            off.SetActive(true);
            _isOn = false;
            Debug.Log("Torcia sbloccata! Premi F per accenderla.");
        }
        else
        {
            // nasconde completamente la torcia
            on.SetActive(false);
            off.SetActive(false);
        }
    }
}