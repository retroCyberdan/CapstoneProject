using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemsUiManager : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    [SerializeField] Image itemImage;
    [SerializeField] TMP_Text itemText;

    [Header("Animazione")]
    [SerializeField] float animationDuration = 0.5f;
    [SerializeField] float moveDistance = 50f;

    [Header("Audio Settings")]
    [SerializeField] AudioClip showClip;
    [SerializeField] AudioClip exitClip;
    [Range(0f, 1f)][SerializeField] float volume = 1f;

    Vector3 startPosition;
    bool itemActive;

    public static ItemsUiManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        startPosition = canvasGroup.transform.localPosition;
    }

    void Start()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        canvasGroup.gameObject.SetActive(false);
    }

    void Update()
    {
        // Chiudi premendo barra spaziatrice
        if (itemActive && Input.GetKeyDown(KeyCode.Space))
        {
            // Suono chiusura
            if (AudioManager.Instance != null && exitClip != null)
                AudioManager.Instance.PlayOneShot(exitClip, transform.position, volume);

            StartCoroutine(AnimateOut());
            itemActive = false;
        }
    }

    public void ShowItem(SO_Items item)
    {
        if (item == null)
        {
            Debug.LogWarning("SO_Items è null!");
            return;
        }

        // Assicurati che il canvas sia attivo PRIMA di fare qualsiasi cosa
        canvasGroup.gameObject.SetActive(true);

        // Porta in primo piano nella hierarchy
        canvasGroup.transform.SetAsLastSibling();

        // Aggiorna i componenti UI con i dati dello ScriptableObject
        if (itemText != null)
        {
            itemText.SetText(item.itemDescription);
        }

        if (itemImage != null && item.itemSprite != null)
        {
            itemImage.sprite = item.itemSprite;
            itemImage.gameObject.SetActive(true);
        }
        else if (itemImage != null)
        {
            itemImage.gameObject.SetActive(false);
        }

        StartCoroutine(ShowItemRoutine());
    }

    IEnumerator ShowItemRoutine()
    {
        itemActive = true;

        // Suono apertura
        if (AudioManager.Instance != null && showClip != null)
            AudioManager.Instance.PlayOneShot(showClip, transform.position, volume);

        yield return StartCoroutine(AnimateIn());
        // Resta aperto finché non premi barra spaziatrice (gestito in Update)
    }

    IEnumerator AnimateIn()
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        Vector3 startPos = startPosition + Vector3.down * moveDistance;
        canvasGroup.transform.localPosition = startPos;

        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / animationDuration;

            canvasGroup.transform.localPosition = Vector3.Lerp(startPos, startPosition, t);
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        canvasGroup.transform.localPosition = startPosition;
        canvasGroup.alpha = 1f;
    }

    IEnumerator AnimateOut()
    {
        Vector3 endPos = startPosition + Vector3.down * moveDistance;

        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / animationDuration;

            canvasGroup.transform.localPosition = Vector3.Lerp(startPosition, endPos, t);
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        canvasGroup.transform.localPosition = startPosition;
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        // Disattiva il canvas dopo l'animazione
        canvasGroup.gameObject.SetActive(false);
    }
}