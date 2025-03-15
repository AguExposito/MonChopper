using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUpItem : MonoBehaviour
{
    [SerializeField] public GameObject prefabItemInventory;
    [SerializeField] private GameObject gridInventory;
    [SerializeField] private GameObject weaponInventory;
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private ItemManager itemManager;
    // Start is called before the first frame update

    void Start()
    {
        inventoryController = FindObjectOfType<InventoryController>(true);
        weaponInventory = GameObject.FindGameObjectWithTag("WeaponInventory");
        itemManager = FindObjectOfType<ItemManager>(true);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            gridInventory=inventoryController.gridInventory; //Gets gridInventory
            AssignItemToInventory();
            Destroy(gameObject);
        }
    }
    public void AssignItemToInventory() {
        for (int i = 0; i < gridInventory.transform.childCount; i++)
        {
            if (gridInventory.transform.GetChild(i).childCount == 0)//checks that gridslot desn't have any child
            {
                WeaponData  weaponData = JsonManager.ScriptableObjectLoadFromFile<WeaponData>("Weapon" + JsonManager.fileId);
                Debug.Log(weaponData + " " + weaponData.itemId);
                itemManager.ReturnToPool(weaponData.itemId.ToString(), weaponData.relatedGameObject);
                GameObject item = Instantiate(prefabItemInventory, gridInventory.transform.GetChild(i)); //instantiates image
                if (item.TryGetComponent<Image>(out Image image)) //Try gets image component else will have default image
                {
                    image.sprite = gameObject.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite;
                }
                break;
            }
        }
    }
}
