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
    [SerializeField] public GameObject weaponItem;
    // Start is called before the first frame update
    private void Awake()
    {
        weapon = GameObject.FindFirstObjectByType<Weapon>();
        InstantiateWeapon();
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void InstantiateWeapon() {
        if (CheckWeaponExistingChild())
        {
            weaponItem = Instantiate(prefabEquippedItem, weapon.transform);

            if (weapon.transform.childCount != 1) //Si no es el único hijo, desactiva la nueva arma
            {
                weaponItem.SetActive(false);
            }
            else
            {
                weaponItem.SetActive(true);
            }
        }
    }

    public bool CheckWeaponExistingChild() //Sirve para no instanciar más de una vez la misma arma
    {
        for (int i = 0; i < weapon.transform.childCount; i++)
        {
            if (weapon.transform.GetChild(i).name == prefabEquippedItem.name + "(Clone)") {
                return false;
            }
        }
        return true;
    }
}
