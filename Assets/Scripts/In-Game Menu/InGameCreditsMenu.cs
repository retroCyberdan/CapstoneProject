using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameCreditsMenu : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // <- premi SPACE per tornare al menu
        {
            ReturnToMainMenu();
            return;
        }
    }

    public void ReturnToMainMenu()
    {
        this.gameObject.SetActive(false);
        SceneManager.LoadScene("MainMenu");
    }
}
