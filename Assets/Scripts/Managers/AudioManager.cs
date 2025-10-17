using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Mixer")]
    public AudioMixer audioMixer; // <- riferimento al mixer per il controllo del volume globale

    [Header("BGM Settings")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;

    [Header("Player Audio Settings")]
    public AudioClip[] footstepSounds; // <- passi su terreno normale
    public AudioClip[] woodFootstepSounds; // <- passi su terreno legnoso
    [Range(0f, 1f)] public float footstepVolume = 0.7f;

    [Header("Running Audio Settings")]
    public AudioClip[] breathingSounds; // <- suoni di affanno durante la corsa
    [Range(0f, 1f)] public float breathingVolume = 0.6f;
    [Range(1f, 2f)] public float runningFootstepPitch = 1.5f; // <- pitch per velocizzare i passi durante la corsa

    [Header("Combat Audio Settings")]
    public AudioClip[] hitSounds; // <- suoni quando il personaggio viene colpito
    [Range(0f, 1f)] public float hitVolume = 0.8f;
    public AudioClip deathSound; // <- suono per la morte del personaggio
    [Range(0f, 1f)] public float deathVolume = 0.9f;

    private AudioSource _currentBGM;
    private AudioSource _breathingSource; // <- source per l'affanno continuo
    private AudioMixerGroup _bgmGroup;
    private AudioMixerGroup _sfxGroup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeMixerGroups();
            LoadVolumeFromPlayerPrefs(); // <- carica tutti i volumi salvati all'avvio
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void InitializeMixerGroups() // <- trova e memorizza i gruppi del mixer
    {
        if (audioMixer != null)
        {
            AudioMixerGroup[] groups = audioMixer.FindMatchingGroups("Master");

            foreach (var group in groups)
            {
                if (group.name == "BGMs") _bgmGroup = group;
                else if (group.name == "SFXs") _sfxGroup = group;
            }
        }
    }

    private void LoadVolumeFromPlayerPrefs() // <- carica i volumi dai PlayerPrefs e li applica al mixer
    {
        float masterVolume = PlayerPrefs.GetFloat(PlayerPrefsKeys.Volume, 0f);
        float bgmVolume = PlayerPrefs.GetFloat(PlayerPrefsKeys.BGMVolume, 0f);
        float sfxVolume = PlayerPrefs.GetFloat(PlayerPrefsKeys.SFXVolume, 0f);

        //SetMixerVolume("Volume", masterVolume);
        //SetMixerVolume("BGMVolume", bgmVolume);
        //SetMixerVolume("SFXVolume", sfxVolume);
    }

    //private void SetMixerVolume(string parameterName, float sliderValue) // <- converte il valore dello slider (0-1) in decibel e lo applica al mixer
    //{
    //    if (audioMixer != null)
    //    {
    //        // Converti il valore lineare (0-1) in decibel (da -80dB a 0dB)
    //        float volumeDB = sliderValue > 0.0001f ? Mathf.Log10(sliderValue) * 20f : -80f;
    //        audioMixer.SetFloat(parameterName, volumeDB);
    //    }
    //}

    public void UpdateVolumeFromPlayerPrefs() // <- metodo pubblico per aggiornare tutti i volumi
    {
        LoadVolumeFromPlayerPrefs();
    }

    public void PlayBGM(AudioClip clip) // <- riproduce musica di sottofondo (menu o gioco)
    {
        if (clip == null) return;

        // ferma la musica precedente se presente
        if (_currentBGM != null)
        {
            _currentBGM.Stop();
            Destroy(_currentBGM.gameObject);
        }

        GameObject bgmObj = new GameObject("BGM_" + clip.name); // <- crea nuovo oggetto per la BGM
        bgmObj.transform.SetParent(transform);
        bgmObj.transform.localPosition = Vector3.zero;

        _currentBGM = bgmObj.AddComponent<AudioSource>();
        _currentBGM.clip = clip;
        _currentBGM.loop = true;
        _currentBGM.outputAudioMixerGroup = _bgmGroup; // <- usa il gruppo Music
        _currentBGM.Play();
    }

    public void PlayMenuMusic() // <- riproduce musica del menu
    {
        PlayBGM(menuMusic);
    }

    public void PlayGameMusic() // <- riproduce musica del gioco
    {
        PlayBGM(gameMusic);
    }

    public void PlayFootstep(Vector2 position, bool isRunning = false, string surfaceType = "Ground") // <- riproduce suono dei passi del player in modo randomico
    {
        AudioClip[] soundArray;

        // seleziona l'array di suoni in base al tipo di superficie
        if (surfaceType == "WoodGround" && woodFootstepSounds != null && woodFootstepSounds.Length > 0)
        {
            soundArray = woodFootstepSounds;
        }
        else if (footstepSounds != null && footstepSounds.Length > 0)
        {
            soundArray = footstepSounds;
        }
        else
        {
            return; // nessun suono disponibile
        }

        AudioClip randomFootstep = soundArray[Random.Range(0, soundArray.Length)]; // <- seleziona un suono casuale dall'array

        if (isRunning)
        {
            // riproduce il passo velocizzato per simulare la corsa
            PlaySoundEffectWithPitch(randomFootstep, position, footstepVolume, runningFootstepPitch);
        }
        else
        {
            PlaySoundEffect(randomFootstep, position, footstepVolume);
        }
    }

    public void StartRunningBreathing(Vector2 position) // <- inizia a riprodurre i suoni di affanno durante la corsa
    {
        if (breathingSounds == null || breathingSounds.Length == 0) return;
        if (_breathingSource != null) return; // <- già in riproduzione

        AudioClip randomBreathing = breathingSounds[Random.Range(0, breathingSounds.Length)];

        GameObject breathingObj = new GameObject("Breathing_Loop");
        breathingObj.transform.position = position;
        breathingObj.transform.SetParent(transform);

        _breathingSource = breathingObj.AddComponent<AudioSource>();
        _breathingSource.clip = randomBreathing;
        _breathingSource.volume = breathingVolume;
        _breathingSource.loop = true;
        _breathingSource.outputAudioMixerGroup = _sfxGroup; // <- usa il gruppo SFX
        _breathingSource.Play();
    }

    public void StopRunningBreathing() // <- ferma i suoni di affanno
    {
        if (_breathingSource != null)
        {
            _breathingSource.Stop();
            Destroy(_breathingSource.gameObject);
            _breathingSource = null;
        }
    }

    public void PlayHit(Vector2 position) // <- riproduce suono quando il personaggio viene colpito
    {
        if (hitSounds == null || hitSounds.Length == 0) return;

        AudioClip randomHit = hitSounds[Random.Range(0, hitSounds.Length)];
        PlaySoundEffect(randomHit, position, hitVolume);
    }

    public void PlayDeath(Vector2 position) // <- riproduce suono della morte del personaggio
    {
        PlaySoundEffect(deathSound, position, deathVolume);
    }

    public void PlaySoundEffect(AudioClip clip, Vector2 position, float volume) // <- funzione generica per riprodurre effetti sonori
    {
        if (clip == null) return;

        GameObject audioObject = new GameObject("Sound_" + clip.name);
        audioObject.transform.position = position;

        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.outputAudioMixerGroup = _sfxGroup; // <- usa il gruppo SFX
        audioSource.Play();

        Destroy(audioObject, clip.length);
    }

    private void PlaySoundEffectWithPitch(AudioClip clip, Vector2 position, float volume, float pitch) // <- riproduce effetti sonori con pitch modificato
    {
        if (clip == null) return;

        GameObject audioObject = new GameObject("Sound_" + clip.name);
        audioObject.transform.position = position;

        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.outputAudioMixerGroup = _sfxGroup; // <- usa il gruppo SFX
        audioSource.Play();

        Destroy(audioObject, clip.length / pitch); // <- adatta la durata in base al pitch
    }

    public void StopBGM() // <- ferma la musica di sottofondo
    {
        if (_currentBGM != null)
        {
            _currentBGM.Stop();
            Destroy(_currentBGM.gameObject);
            _currentBGM = null;
        }
    }

    public void PlayOneShot(AudioClip clip, Vector2 position, float volume = 1f) // <- metodo generico per riprodurre qualsiasi suono
    {
        PlaySoundEffect(clip, position, volume);
    }
}