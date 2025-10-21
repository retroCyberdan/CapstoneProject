using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Tutorials : MonoBehaviour
{
    [SerializeField] AudioClip showClip;
    [SerializeField] AudioClip exitClip;
    [Range(0f, 1f)][SerializeField] float volume = 1f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // <- premi SPAZIO per tornare al gioco
        {
            if (AudioManager.Instance != null && showClip != null) AudioManager.Instance.PlayOneShot(exitClip, transform.position, volume);

            ReturnToGame();
            return;
        }
    }

    public void ReturnToGame()
    {
        Destroy(this.gameObject);
        //this.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        if (AudioManager.Instance != null && showClip != null) AudioManager.Instance.PlayOneShot(showClip, transform.position, volume);
    }
}
