using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IntroManager : MonoBehaviour
{
    [Header("Impostazioni Immagini")]
    [Tooltip("Array di sprite da mostrare in sequenza")]
    public Sprite[] immaginiIntro;

    [Tooltip("Durata di visualizzazione della prima immagine")]
    public float durataPrimaImmagine = 3f;

    [Tooltip("Durata di visualizzazione della seconda immagine (e tutte le successive)")]
    public float durataSecondaImmagine = 3f;

    [Tooltip("Durata del fade tra le immagini (in secondi)")]
    public float durataFade = 1f;

    [Header("Impostazioni Audio")]
    [Tooltip("Array di clip audio da riprodurre in sequenza")]
    public AudioClip[] clipAudio;

    [Header("Riferimenti UI")]
    [Tooltip("Canvas che contiene l'intro (verrà distrutto alla fine)")]
    public Canvas canvasIntro;

    private Image _pannelloNero;
    private Image _immagineCorrente;
    private Image _immagineProssima;
    private Image _pannelloBianco;
    private AudioSource _audioSource;
    private int _indiceAudioCorrente = 0;

    void Awake()
    {
        // controlla se l'intro è già stata vista leggendo direttamente dal file (questo avviene prima che SaveSystem.Instance sia disponibile)
        if (SaveSystem.CheckIntroVistaFromFile())
        {
            Debug.Log("Intro già vista in precedenza, salto l'intro");
            Destroy(gameObject);
            return;
        }

        // crea il canvas se non esiste
        if (canvasIntro == null)
        {
            GameObject canvasObj = new GameObject("IntroCanvas");
            canvasIntro = canvasObj.AddComponent<Canvas>();
            canvasIntro.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasIntro.sortingOrder = 999;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // crea il pannello nero di sfondo (sempre visibile)
        GameObject panNero = new GameObject("PannelloNero");
        panNero.transform.SetParent(canvasIntro.transform, false);
        _pannelloNero = panNero.AddComponent<Image>();
        _pannelloNero.color = Color.black;

        RectTransform rtNero = _pannelloNero.GetComponent<RectTransform>();
        rtNero.anchorMin = Vector2.zero;
        rtNero.anchorMax = Vector2.one;
        rtNero.sizeDelta = Vector2.zero;
        rtNero.anchoredPosition = Vector2.zero;

        // crea la prima immagine
        GameObject img1 = new GameObject("ImmagineCorrente");
        img1.transform.SetParent(canvasIntro.transform, false);
        _immagineCorrente = img1.AddComponent<Image>();
        _immagineCorrente.color = Color.white;

        RectTransform rt1 = _immagineCorrente.GetComponent<RectTransform>();
        rt1.anchorMin = Vector2.zero;
        rt1.anchorMax = Vector2.one;
        rt1.sizeDelta = Vector2.zero;
        rt1.anchoredPosition = Vector2.zero;

        // crea la seconda immagine (per il crossfade)
        GameObject img2 = new GameObject("ImmagineProssima");
        img2.transform.SetParent(canvasIntro.transform, false);
        _immagineProssima = img2.AddComponent<Image>();
        _immagineProssima.color = new Color(1, 1, 1, 0);

        RectTransform rt2 = _immagineProssima.GetComponent<RectTransform>();
        rt2.anchorMin = Vector2.zero;
        rt2.anchorMax = Vector2.one;
        rt2.sizeDelta = Vector2.zero;
        rt2.anchoredPosition = Vector2.zero;

        // crea il pannello bianco per il fade finale
        GameObject panObj = new GameObject("PannelloBianco");
        panObj.transform.SetParent(canvasIntro.transform, false);
        _pannelloBianco = panObj.AddComponent<Image>();
        _pannelloBianco.color = new Color(1, 1, 1, 0);

        RectTransform rtBianco = _pannelloBianco.GetComponent<RectTransform>();
        rtBianco.anchorMin = Vector2.zero;
        rtBianco.anchorMax = Vector2.one;
        rtBianco.sizeDelta = Vector2.zero;
        rtBianco.anchoredPosition = Vector2.zero;

        _audioSource = gameObject.AddComponent<AudioSource>(); // <- crea l'AudioSource

        StartCoroutine(IntroSequence()); // <- avvia la sequenza intro
    }

    IEnumerator IntroSequence()
    {
        if (clipAudio.Length > 0) PlayNextAudio(); // <- avvia la riproduzione audio

        if (immaginiIntro.Length == 0)
        {
            yield return WhiteFadeIn();
            CheckAsViewed();
            Destroy(canvasIntro.gameObject);
            Destroy(gameObject);
            yield break;
        }

        // mostra la prima immagine
        _immagineCorrente.sprite = immaginiIntro[0];
        yield return new WaitForSeconds(durataPrimaImmagine);

        // loop per le immagini successive
        for (int i = 1; i < immaginiIntro.Length; i++)
        {
            // prepara la prossima immagine
            _immagineProssima.sprite = immaginiIntro[i];

            // fade out immagine corrente / Fade in prossima immagine
            yield return CrossFade();

            // scambia i riferimenti
            Image temp = _immagineCorrente;
            _immagineCorrente = _immagineProssima;
            _immagineProssima = temp;

            // mostra la seconda immagine (e tutte le successive)
            yield return new WaitForSeconds(durataSecondaImmagine);
        }

        // fade in bianco finale
        yield return WhiteFadeIn();

        // attende che tutti gli audio finiscano
        while (_audioSource.isPlaying)
        {
            yield return null;
        }

        CheckAsViewed(); // <- segna l'intro come vista

        // distrugge il canvas e questo script
        Destroy(canvasIntro.gameObject);
        Destroy(gameObject);
    }

    IEnumerator CrossFade()
    {
        float tempoTrascorso = 0f;

        while (tempoTrascorso < durataFade)
        {
            tempoTrascorso += Time.deltaTime;
            float t = tempoTrascorso / durataFade;

            // fade out immagine corrente
            _immagineCorrente.color = new Color(1, 1, 1, 1 - t);

            // fade in prossima immagine
            _immagineProssima.color = new Color(1, 1, 1, t);

            yield return null;
        }

        _immagineCorrente.color = new Color(1, 1, 1, 0);
        _immagineProssima.color = Color.white;
    }

    IEnumerator WhiteFadeIn()
    {
        float tempoTrascorso = 0f;

        while (tempoTrascorso < durataFade)
        {
            tempoTrascorso += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, tempoTrascorso / durataFade);
            _pannelloBianco.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        _pannelloBianco.color = Color.white;
    }

    void PlayNextAudio()
    {
        if (_indiceAudioCorrente < clipAudio.Length)
        {
            _audioSource.clip = clipAudio[_indiceAudioCorrente];
            _audioSource.Play();
            _indiceAudioCorrente++;

            StartCoroutine(WaitUntilAudio());
        }
    }

    IEnumerator WaitUntilAudio()
    {
        yield return new WaitWhile(() => _audioSource.isPlaying);
        PlayNextAudio();
    }

    void CheckAsViewed()
    {
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.SetIntroAsViewed();
            Debug.Log("Intro completata e segnata come vista");
        }
    }
}