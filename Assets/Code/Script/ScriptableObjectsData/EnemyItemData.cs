using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyItem", menuName = "Item/EnemyItem", order = 1)]
public class EnemyItemData : ItemData
{
    public enum bodyPart { Head, Arm, Leg, Ear, Wing, Claw, Horn, Skin, Other }
    [Header("EnemyItemInfo")]
    public bodyPart bodyPartType;
    public Sprite iconSprite;
}