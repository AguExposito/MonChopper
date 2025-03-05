using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{    
    [SerializeField] public enum GridType { Equippable, Inventory, Mon, Armor };
    [SerializeField] public GridType gridType;
    private void Start()
    {
        switch (transform.parent.name) {
            case "GridEquipped": { gridType = GridType.Equippable; }break;
            case "GridInventory": { gridType = GridType.Inventory; } break;
            case "GridArmor": { gridType = GridType.Armor; } break;
            case "GridMons": { gridType = GridType.Mon; } break;
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        
        if (transform.childCount == 0)
        {            
            GameObject dropped = eventData.pointerDrag;
            DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();

            

            if (CanBeReparented(draggableItem)) {
                draggableItem.parenAfterDrag = transform;
            }
        }
    }
    private bool CanBeReparented(DraggableItem dgi) {
        switch (dgi.itemType) {
            case DraggableItem.ItemType.Weapon: {
                    if (gridType == GridType.Equippable || gridType == GridType.Inventory) { return true; }
                    else { return false; }
                }
            case DraggableItem.ItemType.Material: {
                    if (gridType == GridType.Equippable || gridType == GridType.Inventory) { return true; }
                    else { return false; }
                }
            case DraggableItem.ItemType.Armor: {
                    if (gridType == GridType.Armor || gridType == GridType.Inventory) { return true; }
                    else { return false; }
                } 
            case DraggableItem.ItemType.Mon: {
                    if (gridType == GridType.Mon || gridType == GridType.Inventory) { return true; }
                    else { return false; }
                } 
            default: return false;
        }
    }
}
