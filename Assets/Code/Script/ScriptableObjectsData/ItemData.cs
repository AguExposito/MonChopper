using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item")]
public class ItemData : ScriptableObject
{
    public enum itemType { weapon, armor, valuable, consumable, pickup, other }
    public enum tier { Common, Rare, SuperRare, Legendary, Mythic }

    [Header("ItemInfo")]
    public itemType item_Type;
    public string itemName;
    public tier itemTier;
    public float sellPrice;
    public string description;
    public GameObject relatedGameObject;

}