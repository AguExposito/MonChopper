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
            GameObject dropped = eventData.pointerDrag; //Dragged gameobject
            DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
            ItemFunctionsUI itemFunctionsUI = dropped.GetComponent<ItemFunctionsUI>();

            if (CanBeReparented(itemFunctionsUI))
            {
                draggableItem.parenAfterDrag = transform;
            }

            if (gridType == GridType.Equippable) //Instancia weapon cunado la equipas
            {
                itemFunctionsUI.InstantiateWeapon();
            }
            else //Destruye weapon cuando lo desequipas
            {
                if (itemFunctionsUI.weapon.transform.childCount != 1)
                {
                    for (int i = 0; i < itemFunctionsUI.weapon.transform.childCount; i++) //Pasa por todos los hijos
                    {
                        if (itemFunctionsUI.weapon.transform.GetChild(i).gameObject == itemFunctionsUI.weaponItem && itemFunctionsUI.weaponItem.activeInHierarchy) //chequea que sea el mismo game object y que esté activo para prevenir bugs
                        {
                            if (i < itemFunctionsUI.weapon.transform.childCount - 1) //si no es el último hijo
                            {
                                itemFunctionsUI.weapon.transform.GetChild(i + 1).gameObject.SetActive(true); Debug.Log("No es el último: "+(i+1));
                            }
                            else { itemFunctionsUI.weapon.transform.GetChild(i - 1).gameObject.SetActive(true); Debug.Log("Es el último: "+(i - 1)); }
                            break;
                        }
                    }
                }
                Destroy(itemFunctionsUI.weaponItem);        
                itemFunctionsUI.weapon.AssignWeaponVariables();
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
