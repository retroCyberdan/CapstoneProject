using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DataSystem : MonoBehaviour
{
    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private TMP_Dropdown _graphicsDropdown;
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    [SerializeField] private Toggle _fullScreenToggle;

    void Start()
    {
        Load();
    }

    public void Load()
    {
        _volumeSlider.value = PlayerPrefs.GetFloat(PlayerPrefsKeys.Volume, .5f);
        _graphicsDropdown.value = PlayerPrefs.GetInt(PlayerPrefsKeys.Graphics, 0);
        _resolutionDropdown.value = PlayerPrefs.GetInt(PlayerPrefsKeys.Resolution, 0);
        _fullScreenToggle.isOn = PlayerPrefs.GetInt(PlayerPrefsKeys.FullScreen, 0) == 1 ? true : false;
    }

    // Queste funzioni vanno collegate agli eventi OnValueChanged nell'Inspector
    public void SetVolume(float volume)
    {
        PlayerPrefs.SetFloat(PlayerPrefsKeys.Volume, volume);
        PlayerPrefs.Save();
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