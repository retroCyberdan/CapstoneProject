using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class SO_Items : ScriptableObject
{
    [Header("Informazioni Item")]
    public Sprite itemSprite;
    [TextArea(3, 10)]
    public string itemDescription;
}