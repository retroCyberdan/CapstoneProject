using UnityEngine;

// Script da mettere nella Scene1 per gestire l'inizializzazione
public class GameSceneInitializer : MonoBehaviour
{
    private void Start()
    {
        // Controlla automaticamente se esiste un salvataggio e caricalo
        if (SaveSystem.SaveFileExists() && SaveSystem.Instance != null)
        {
            // Carica il salvataggio solo se esiste
            SaveSystem.Instance.Load();
            Debug.Log("Salvataggio caricato automaticamente");
        }
        else
        {
            Debug.Log("Nuova partita iniziata - nessun salvataggio trovato");
        }
    }
}