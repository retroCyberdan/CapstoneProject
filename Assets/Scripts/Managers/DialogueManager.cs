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
    [SerializeField] float moveDistance = 50f; // distanza movimento (in pixel)

    Story story;
    bool dialogueActive;
    Vector3 startPosition;

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
            StartCoroutine(AnimateOut());
            dialogueActive = false;
        }
    }

    public void StartDialogue(TextAsset dialogue)
    {
        story = new Story(dialogue.text);
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

            if (story.currentChoices.Count == 0)
            {
                HideButtons();
            }
            else
            {
                ShowButtons();
            }
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

        // Posizione iniziale (sotto)
        Vector3 startPos = startPosition + Vector3.down * moveDistance;
        canvasGroup.transform.localPosition = startPos;

        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;

            // Lerp per movimento
            canvasGroup.transform.localPosition = Vector3.Lerp(startPos, startPosition, t);
            // Lerp per fade
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        // Assicura valori finali
        canvasGroup.transform.localPosition = startPosition;
        canvasGroup.alpha = 1f;
    }

    IEnumerator AnimateOut()
    {
        // Posizione finale (sotto)
        Vector3 endPos = startPosition + Vector3.down * moveDistance;

        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;

            // Lerp per movimento
            canvasGroup.transform.localPosition = Vector3.Lerp(startPosition, endPos, t);
            // Lerp per fade
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        // Assicura valori finali
        canvasGroup.transform.localPosition = startPosition;
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    public void OnComplete()
    {
        gameObject.SetActive(false);
    }
}