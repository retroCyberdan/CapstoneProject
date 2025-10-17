using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;

    private void Start()
    {
        resolutions = Screen.resolutions; // <- acquisisco tutte le risoluzioni supportate

        resolutionDropdown.ClearOptions(); // <- pulisco il dropdown

        List<string> options = new List<string>(); // <- lista di opzioni per il dropdown (poiché non supporta array)

        int currentResolutionIndex = 0; // <- indice della risoluzione attuale
        for (int i = 0; i < resolutions.Length; i++) // <- ciclo per ogni risoluzione
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height) // <- se la risoluzione è quella attuale
                currentResolutionIndex = i;
        }

        resolutionDropdown.AddOptions(options); // <- aggiungo le opzioni al dropdown
        resolutionDropdown.value = currentResolutionIndex; // <- imposto il valore del dropdown alla risoluzione attuale
        resolutionDropdown.RefreshShownValue(); // <- aggiorno il valore mostrato
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
    }

    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGMVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
    }


    public void SetGraphicQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
