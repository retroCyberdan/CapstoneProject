using UnityEngine;

public class UI_CanvasTrigger : MonoBehaviour
{
    [SerializeField] private Canvas _targetCanvas;
    [SerializeField] private string _playerTag = "Player";

    private void Awake()
    {
        if (_targetCanvas != null) _targetCanvas.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_playerTag) && _targetCanvas != null) _targetCanvas.gameObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_playerTag) && _targetCanvas != null) _targetCanvas.gameObject.SetActive(false);
    }
}