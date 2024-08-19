using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFunctionsUI : MonoBehaviour
{
    [SerializeField] public enum ItemType { Weapon, Material, Mon, Armor };
    [SerializeField] public ItemType itemType;
    [Header("References")]
    [SerializeField] public Weapon weapon;
    [SerializeField] public GameObject prefabDropItem;
    [SerializeField] public GameObject prefabEquippedItem;
    // Start is called before the first frame update
    private void Awake()
    {
        weapon = GameObject.FindFirstObjectByType<Weapon>();
       // InstantiateWeapon();
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void InstantiateWeapon() {
        GameObject weaponItem=Instantiate(prefabEquippedItem,weapon.transform);
        
        if (weapon.transform.childCount != 1) //Si no es el único hijo, desactiva la nueva arma
        {
            weaponItem.SetActive(false);
        }
        else {
            weaponItem.SetActive(true);
        }
        
    }
}
