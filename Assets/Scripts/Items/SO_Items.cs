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
}