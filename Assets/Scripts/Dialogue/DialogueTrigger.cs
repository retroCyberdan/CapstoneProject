using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Ink.Runtime;

// by il grandissimo prof. Luca Villanini (1h37m M7S1G4)

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] Canvas popUpCanvas;
    [SerializeField] TextAsset dialogue;

    bool isPlayerInRange;
    bool isDialogueStarted;

    private void Awake()
    {
        if (popUpCanvas != null) popUpCanvas.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            DialogueManager.Instance.canvasGroup.gameObject.SetActive(true);
            DialogueManager.Instance.StartDialogue(dialogue);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            popUpCanvas.gameObject.SetActive(true);
        }
    }
        void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            popUpCanvas.gameObject.SetActive(false);
            DialogueManager.Instance.canvasGroup.gameObject.SetActive(false);
        }
    }
}
