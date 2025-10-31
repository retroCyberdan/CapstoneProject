using UnityEngine;

public class ShiftingObjects : MonoBehaviour
{
    [Header("Impostazioni Movimento")]
    [SerializeField] private bool _isLeftShifting = true;
    [SerializeField] private float _speed = 2f;

    [Header("Attivazione")]
    [SerializeField] private bool _isActive = false; // Controlla se l'oggetto deve muoversi

    void Update()
    {
        // muove solo se attivo
        if (!_isActive) return;

        // calcola la direzione in base al trigger
        float direction = _isLeftShifting ? -1f : 1f;

        // trasla l'oggetto sull'asse X
        transform.Translate(Vector3.right * direction * _speed * Time.deltaTime);
    }

    public void ActivateMovement() // <- metodo pubblico per attivare il movimento (chiamato dal DialogueManager)
    {
        _isActive = true;
        Debug.Log($"{gameObject.name} inizia a muoversi!");
    }

    public void DeactivateMovement() // <- metodo pubblico per disattivare il movimento
    {
        _isActive = false;
        Debug.Log($"{gameObject.name} smette di muoversi!");
    }
}