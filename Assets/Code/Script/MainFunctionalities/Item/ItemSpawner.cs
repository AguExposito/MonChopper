using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemSpawner : MonoBehaviour
{
    public static GameObject player;
    public ItemData data;
    private void Awake()
    {
    }
    public void SpawnItem(Vector3 originPosition, Sprite spriteItemInventory,ItemData.itemType itemType, float itemLaunchForce=1.5f)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        GameObject item = Instantiate(gameObject, originPosition, Quaternion.identity);
        if (item.TryGetComponent<Rigidbody>(out Rigidbody rb)) //applies explosion force on spawn
        {
            // Calcula la dirección opuesta al jugador
            Vector3 forceDirection = (originPosition - player.transform.position).normalized;

            // Añade una componente hacia arriba a la dirección
            Vector3 launchDirection = (forceDirection + Vector3.up).normalized;

            // Aplica la fuerza con un factor multiplicador para ajustar la magnitud
            rb.AddForce(itemLaunchForce * launchDirection, ForceMode.Impulse);
        }
        if (item.transform.GetChild(0).TryGetComponent<Animator>(out Animator animator)) //Sets animation active
        {
            animator.SetLayerWeight(1, 1); //Changes weight
        }
        if (item.transform.GetChild(0).GetChild(0).TryGetComponent<SpriteRenderer>(out SpriteRenderer renderer)) //Try gets image component else will have default image
        {
            renderer.sprite = spriteItemInventory;
        }
        AssignScriptableObject(itemType, spriteItemInventory);
    }

    public void AssignScriptableObject(ItemData.itemType itemType, Sprite spriteItemInventory) {
        switch (itemType)
        {
            case ItemData.itemType.weapon:
                {
                    WeaponData weaponData = (WeaponData)Instantiate(data);
                    weaponData.weaponType=WeaponData.weapon.Pistol;
                    weaponData.relatedSprite= spriteItemInventory;
                    weaponData.GenerateRandomWeapon(weaponData.weaponType);
                    string jsonData = JsonUtility.ToJson(weaponData,true);
                    JsonManager.SaveToFile("Weapon"+JsonManager.fileId, weaponData);

                }
                break;
            case ItemData.itemType.mateial:
                {
                    EnemyItemData enemyItemData = Instantiate((EnemyItemData)data);
                }
                break;
            case ItemData.itemType.valuable:
                {
                    EnemyItemData enemyItemData = Instantiate((EnemyItemData)data);
                }
                break;
            default: 
                { 
                    Instantiate(data); 
                } break;
        
        }
    }
}
