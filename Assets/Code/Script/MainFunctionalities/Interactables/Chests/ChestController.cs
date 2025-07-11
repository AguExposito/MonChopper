using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    public GameObject prefabDroppedObject;
    public Sprite spriteItemInventory;
    bool open=false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!open)
            {
                GetComponent<Animator>().Play("OpenChest");
                prefabDroppedObject.GetComponent<ItemSpawner>().SpawnItem(transform.GetChild(2).transform.position, spriteItemInventory,ItemData.itemType.weapon,2);
                open= true;
            }
        }
    }
}
