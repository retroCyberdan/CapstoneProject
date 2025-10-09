using UnityEngine;
using TMPro;

public class CreditsMenu : MonoBehaviour
{
    public GameObject mainMenu; // Riferimento al menu principale

    void Update()
    {
        // Premi ESC per tornare al menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToMainMenu();
            return;
        }
    }

    public void ReturnToMainMenu()
    {
        this.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
    }
}