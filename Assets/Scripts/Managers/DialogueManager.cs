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
    [SerializeField] float moveDistance = 50f; // <- distanza movimento (in pixel)

    [Header("Save System References")]
    [SerializeField] SaveSystem saveSystem;
    [SerializeField] HealthSystem healthSystem;

    [Header("Oggetti da attivare")]
    [SerializeField] GameObject[] oggettiDaAttivare;

    Story story;
    bool dialogueActive;
    Vector3 startPosition;
    Dictionary<string, object> savedVariables = new Dictionary<string, object>(); // Salva solo le variabili

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
            story.variablesState[variable.Key] = variable.Value;
        }

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
                // formato tag: attiva_oggetto:0 (dove 0 è l'indice dell'array)
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

                // tag per salvare e curare
                else if (tag == "save") Save();

                else if (tag == "heal") Heal();
            }
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

        // posizione iniziale (sotto)
        Vector3 startPos = startPosition + Vector3.down * moveDistance;
        canvasGroup.transform.localPosition = startPos;

        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;

            canvasGroup.transform.localPosition = Vector3.Lerp(startPos, startPosition, t); // <- lerp per movimento

            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t); // <- lerp per fade

            yield return null;
        }

        // Assicura valori finali
        canvasGroup.transform.localPosition = startPosition;
        canvasGroup.alpha = 1f;
    }

    IEnumerator AnimateOut()
    {
        // posizione finale (sotto)
        Vector3 endPos = startPosition + Vector3.down * moveDistance;

        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;

            canvasGroup.transform.localPosition = Vector3.Lerp(startPosition, endPos, t); // <- lerp per movimento

            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t); // <- lerp per fade

            yield return null;
        }

        // assicura valori finali
        canvasGroup.transform.localPosition = startPosition;
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    IEnumerator AnimateOutAndActivate()
    {
        // salva le variabili prima di chiudere
        if (story != null)
        {
            savedVariables.Clear();
            foreach (string variableName in story.variablesState)
            {
                savedVariables[variableName] = story.variablesState[variableName];
            }
        }

        yield return StartCoroutine(AnimateOut()); // <- esegui l'animazione di uscita

        yield return new WaitForSeconds(0.5f); // <- aspetta un po' di tempo prima di attivare gli oggetti (modifica questo valore (in sec))

        CheckAndActivateObjects(); // <- dopo l'animazione e l'attesa, attiva gli oggetti
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