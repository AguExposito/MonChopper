using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUpItem : MonoBehaviour
{
    [SerializeField] public GameObject prefabItemInventory;
    [SerializeField] private GameObject gridInventory;
    // Start is called before the first frame update

    void Start()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            gridInventory=other.transform.Find("ScreenUI").GetComponent<MenuController>().inventory.GetComponent<InventoryController>().gridInventory; //Gets gridInventory
            for (int i = 0; i < gridInventory.transform.childCount; i++)
            {
                if (gridInventory.transform.GetChild(i).childCount == 0)//checks that gridslot desn't have any child
                {
                    GameObject item = Instantiate(prefabItemInventory, gridInventory.transform.GetChild(i)); //instantiates image
                    if (item.TryGetComponent<Image>(out Image image)) //Try gets image component else will have default image
                    {
                        image.sprite = gameObject.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite;
                    }
                    break;
                }
            }
            Destroy(gameObject);
        }
    }
}
