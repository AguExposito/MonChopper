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
            ItemFunctionsUI itemFunctionsUI = dropped.GetComponent<ItemFunctionsUI>();            

            if (CanBeReparented(itemFunctionsUI)) {
                draggableItem.parenAfterDrag = transform;
            }
        }
    }
    private bool CanBeReparented(ItemFunctionsUI ifUI) {
        switch (ifUI.itemType) {
            case ItemFunctionsUI.ItemType.Weapon: {
                    if (gridType == GridType.Equippable || gridType == GridType.Inventory) { return true; }
                    else { return false; }
                } break;
            case ItemFunctionsUI.ItemType.Material: {
                    if (gridType == GridType.Equippable || gridType == GridType.Inventory) { return true; }
                    else { return false; }
                } break;
            case ItemFunctionsUI.ItemType.Armor: {
                    if (gridType == GridType.Armor || gridType == GridType.Inventory) { return true; }
                    else { return false; }
                } break;
            case ItemFunctionsUI.ItemType.Mon: {
                    if (gridType == GridType.Mon || gridType == GridType.Inventory) { return true; }
                    else { return false; }
                } break;
            default: return false;
        }
    }
}
