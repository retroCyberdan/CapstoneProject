using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DataSystem : MonoBehaviour
{
    [SerializeField] private Slider _volumeSlider; // <- volume Master
    [SerializeField] private Slider _bgmVolumeSlider; // <- volume BGM (opzionale)
    [SerializeField] private Slider _sfxVolumeSlider; // <- volume SFX (opzionale)
    [SerializeField] private TMP_Dropdown _graphicsDropdown;
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    [SerializeField] private Toggle _fullScreenToggle;

    void Start()
    {
        Load();
    }

    public void Load()
    {
        _volumeSlider.value = PlayerPrefs.GetFloat(PlayerPrefsKeys.Volume, 0f);

        // carica i volumi separati solo se gli slider esistono
        if (_bgmVolumeSlider != null) _bgmVolumeSlider.value = PlayerPrefs.GetFloat(PlayerPrefsKeys.BGMVolume, 0f);

        if (_sfxVolumeSlider != null) _sfxVolumeSlider.value = PlayerPrefs.GetFloat(PlayerPrefsKeys.SFXVolume, 0f);

        _graphicsDropdown.value = PlayerPrefs.GetInt(PlayerPrefsKeys.Graphics, 0);
        _resolutionDropdown.value = PlayerPrefs.GetInt(PlayerPrefsKeys.Resolution, 0);
        _fullScreenToggle.isOn = PlayerPrefs.GetInt(PlayerPrefsKeys.FullScreen, 0) == 1 ? true : false;
    }

    // queste funzioni vanno collegate agli eventi OnValueChanged nell'Inspector
    public void SetVolume(float volume)
    {
        PlayerPrefs.SetFloat(PlayerPrefsKeys.Volume, volume);
        PlayerPrefs.Save();

        // notifica l'AudioManager del cambio volume
        if (AudioManager.Instance != null) AudioManager.Instance.UpdateVolumeFromPlayerPrefs();
    }

    public void SetBGMVolume(float volume) // <- nuovo metodo per volume musica separato
    {
        PlayerPrefs.SetFloat(PlayerPrefsKeys.BGMVolume, volume);
        PlayerPrefs.Save();

        if (AudioManager.Instance != null) AudioManager.Instance.UpdateVolumeFromPlayerPrefs();
    }

    public void SetSFXVolume(float volume) // <- nuovo metodo per volume SFX separato
    {
        PlayerPrefs.SetFloat(PlayerPrefsKeys.SFXVolume, volume);
        PlayerPrefs.Save();

        if (AudioManager.Instance != null) AudioManager.Instance.UpdateVolumeFromPlayerPrefs();
    }

    public void SetGraphics(int graphicsIndex)
    {
        PlayerPrefs.SetInt(PlayerPrefsKeys.Graphics, graphicsIndex);
        PlayerPrefs.Save();
    }

    public void SetResolution(int resolutionIndex)
    {
        PlayerPrefs.SetInt(PlayerPrefsKeys.Resolution, resolutionIndex);
        PlayerPrefs.Save();
    }

    public void SetFullScreen(bool isFullScreen)
    {
        PlayerPrefs.SetInt(PlayerPrefsKeys.FullScreen, isFullScreen ? 1 : 0);
        PlayerPrefs.Save();
    }
}