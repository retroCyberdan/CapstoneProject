using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcAnimationsController : MonoBehaviour
{
    private Animator _animator;
    private DialogueTrigger _dialogueTrigger;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _dialogueTrigger = GetComponent<DialogueTrigger>();
    }

    void Update()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.canvasGroup != null) // <- controlla se il DialogueManager è attivo (dialogo in corso)
        {
            bool isDialogueActive = DialogueManager.Instance.canvasGroup.gameObject.activeSelf;
            _animator.SetBool("isTalking", isDialogueActive);
        }
        else
        {
            _animator.SetBool("isTalking", false);
        }
    }
}