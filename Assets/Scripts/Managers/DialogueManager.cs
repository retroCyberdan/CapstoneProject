using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;

public class DialogueManager : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    [SerializeField] TMP_Text dialogueText;
    [SerializeField] Button[] myButtons;

    [Header("Animazione")]
    [SerializeField] float animationDuration = 0.5f;
    [SerializeField] float moveDistance = 50f;

    [Header("Save System References")]
    [SerializeField] SaveSystem saveSystem;
    [SerializeField] HealthSystem healthSystem;

    [Header("Oggetti da attivare")]
    [Tooltip("Array di SO_Items da poter attivare tramite dialoghi INK")]
    [SerializeField] GameObject[] oggettiDaAttivare;

    [Header("Oggetti da aggiungere all'inventario")]
    [Tooltip("Array di SO_Items da poter aggiungere tramite dialoghi INK")]
    [SerializeField] SO_Items[] oggettiDaAggiungere;

    [Header("Oggetti da spostare")]
    [Tooltip("Oggetti con script ShiftingObjects da attivare")]
    [SerializeField] ShiftingObjects[] shiftingObjects;

    [Header("Audio per Dialoghi")]
    [Tooltip("Array di AudioClip da riprodurre durante i dialoghi")]
    [SerializeField] AudioClip[] dialogueAudioClips;
    [Range(0f, 1f)]
    [SerializeField] float dialogueAudioVolume = 0.8f;

    [Header("Camera Controller")]
    [SerializeField] ThirdCameraController cameraController;

    Story story;
    bool dialogueActive;
    Vector3 startPosition;
    Dictionary<string, object> savedVariables = new Dictionary<string, object>();

    public static DialogueManager Instance { get; private set; }

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
    }

    void Update()
    {
        if (dialogueActive && story.canContinue && Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextLine();
        }
        else if (dialogueActive && !story.canContinue && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(AnimateOutAndActivate());
            dialogueActive = false;
        }
    }

    public void StartDialogue(TextAsset dialogue)
    {
        story = new Story(dialogue.text);

        // ripristina le variabili salvate
        foreach (var variable in savedVariables)
        {
            // prova a settare la variabile, se esiste in questo dialogo
            try
            {
                story.variablesState[variable.Key] = variable.Value;
            }
            catch
            {
                // ignora se la variabile non esiste in questo dialogo specifico
            }
        }

        if (DialogueInventoryBridge.Instance != null) DialogueInventoryBridge.Instance.BindInventoryFunctionsToStory(story); // <- collega le funzioni inventario a INK

        if (cameraController != null) cameraController.ShowCursor(); // <- mostra il cursore quando si apre il dialogo

        StartCoroutine(AnimateIn());
        ShowNextLine();
        dialogueActive = true;
    }

    private void ShowNextLine()
    {
        if (story.canContinue)
        {
            string text = story.Continue();
            dialogueText.SetText(text);

            CheckAndActivateObjects(); // <- controlla e processa i tag SUBITO dopo ogni riga

            if (story.currentChoices.Count == 0) HideButtons();

            else ShowButtons();
        }
        else
        {
            dialogueText.SetText("");
            HideButtons();
        }
    }

    private void ShowButtons()
    {
        for (int i = 0; i < myButtons.Length; i++)
        {
            var button = myButtons[i];
            button.onClick.RemoveAllListeners();
            button.gameObject.SetActive(i < story.currentChoices.Count);
            if (i < story.currentChoices.Count)
            {
                var choice = story.currentChoices[i];
                button.GetComponentInChildren<TMP_Text>().SetText(choice.text);
                button.onClick.AddListener(() =>
                {
                    story.ChooseChoiceIndex(choice.index);
                    ShowNextLine();
                });
            }
        }
    }

    private void CheckAndActivateObjects()
    {
        if (story.currentTags.Count > 0)
        {
            foreach (string tag in story.currentTags)
            {
                if (tag.StartsWith("attiva_oggetto:"))
                {
                    string[] tagParts = tag.Split(':');
                    if (tagParts.Length > 1 && int.TryParse(tagParts[1], out int index))
                    {
                        if (index >= 0 && index < oggettiDaAttivare.Length && oggettiDaAttivare[index] != null)
                        {
                            oggettiDaAttivare[index].SetActive(true);
                        }
                    }
                }
                else if (tag.StartsWith("attiva_shifting"))
                {
                    // attiva tutti gli oggetti ShiftingObjects
                    foreach (ShiftingObjects obj in shiftingObjects)
                    {
                        if (obj != null)
                        {
                            obj.ActivateMovement();
                        }
                    }
                    Debug.Log("Tutti gli oggetti ShiftingObjects attivati!");
                }
                else if (tag.StartsWith("aggiungi_oggetto:"))
                {
                    string[] tagParts = tag.Split(':');
                    if (tagParts.Length > 1 && int.TryParse(tagParts[1], out int index))
                    {
                        if (index >= 0 && index < oggettiDaAggiungere.Length && oggettiDaAggiungere[index] != null)
                        {
                            if (InventoryManager.Instance != null)
                            {
                                InventoryManager.Instance.AddItem(oggettiDaAggiungere[index]);
                                Debug.Log($"Aggiunto {oggettiDaAggiungere[index].name} all'inventario tramite dialogo");
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"Indice {index} non valido per oggettiDaAggiungere!");
                        }
                    }
                }
                else if (tag.StartsWith("rimuovi_oggetto:"))
                {
                    string itemID = tag.Substring("rimuovi_oggetto:".Length);
                    if (InventoryManager.Instance != null)
                    {
                        SO_Items itemToRemove = InventoryManager.Instance.GetItemByID(itemID);
                        if (itemToRemove != null)
                        {
                            InventoryManager.Instance.RemoveItem(itemToRemove);

                            // Setta un flag globale per tracciare che l'oggetto è stato posizionato
                            if (DialogueGlobalFlagsManager.Instance != null)
                            {
                                DialogueGlobalFlagsManager.Instance.SetFlag($"{itemID}_posizionato", true);
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"Oggetto con ID '{itemID}' non trovato nell'inventario!");
                        }
                    }
                }
                else if (tag.StartsWith("suono:"))
                {
                    // Gestisce i suoni dei dialoghi
                    string[] tagParts = tag.Split(':');
                    if (tagParts.Length > 1 && int.TryParse(tagParts[1], out int audioIndex))
                    {
                        PlayDialogueAudio(audioIndex);
                    }
                }
                else if (tag == "save") Save();
                else if (tag == "heal") Heal();
            }
        }
    }

    private void PlayDialogueAudio(int index)
    {
        if (dialogueAudioClips == null || dialogueAudioClips.Length == 0)
        {
            Debug.LogWarning("Nessun AudioClip disponibile nell'array dialogueAudioClips!");
            return;
        }

        if (index < 0 || index >= dialogueAudioClips.Length)
        {
            Debug.LogWarning($"Indice audio {index} fuori range! Array contiene {dialogueAudioClips.Length} elementi.");
            return;
        }

        AudioClip clip = dialogueAudioClips[index];
        if (clip == null)
        {
            Debug.LogWarning($"AudioClip all'indice {index} è null!");
            return;
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySoundEffect(clip, Vector2.zero, dialogueAudioVolume); // <- riproduce il suono nella posizione del canvas (o Vector2.zero)
            Debug.Log($"Riprodotto audio: {clip.name}");
        }
        else
        {
            Debug.LogWarning("AudioManager.Instance non trovato!");
        }
    }

    private void HideButtons()
    {
        foreach (var button in myButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    public void Show()
    {
        StartCoroutine(AnimateIn());
    }

    public void Hide()
    {
        StartCoroutine(AnimateOut());
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
            elapsedTime += Time.deltaTime;
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
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;

            canvasGroup.transform.localPosition = Vector3.Lerp(startPosition, endPos, t);
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        canvasGroup.transform.localPosition = startPosition;
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        if (cameraController != null) cameraController.HideCursor(); // <- nasconde il cursore quando si chiude il dialogo
    }

    IEnumerator AnimateOutAndActivate()
    {
        if (story != null)
        {
            savedVariables.Clear();
            foreach (string variableName in story.variablesState)
            {
                savedVariables[variableName] = story.variablesState[variableName];
            }
        }

        yield return StartCoroutine(AnimateOut());
        yield return new WaitForSeconds(0.5f);
        //CheckAndActivateObjects();    DAVA PROBLEMI SE CHIAMATO QUI
    }

    public void OnComplete()
    {
        gameObject.SetActive(false);
    }

    private void Save()
    {
        if (saveSystem != null) saveSystem.Save();

        else Debug.LogWarning("SaveSystem non assegnato nel DialogueManager!");
    }

    private void Heal()
    {
        if (healthSystem != null) healthSystem.HealToMax();

        else Debug.LogWarning("HealthSystem non assegnato nel DialogueManager!");
    }
}