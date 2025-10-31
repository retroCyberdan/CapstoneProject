using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class SO_Items : ScriptableObject
{
    [Header("Identificazione")]
    [Tooltip("ID univoco per il salvataggio - DEVE essere unico per ogni oggetto! Es: 'key_001', 'potion_health', 'note_diary'")]
    public string itemID;

    [Header("Informazioni Item")]
    public Sprite itemSprite;
    [TextArea(3, 10)]
    public string itemDescription;

    [Header("Audio Settings")]
    [Tooltip("Abilita la riproduzione del suono per questo oggetto")]
    public bool useObjectSound = false;
    [Tooltip("Suono riprodotto quando viene mostrata la UI di questo oggetto")]
    public AudioClip objectSound;
    [Range(0f, 1f)]
    public float soundVolume = 1f;
}