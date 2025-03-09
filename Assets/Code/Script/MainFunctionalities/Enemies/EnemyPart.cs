using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EnemyPart : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] public Enemy enemyScript;
    [SerializeField] public GameObject prefabDroppedItem;
    [SerializeField] public GameObject prefabItemInventory;
    [SerializeField] public Sprite spriteItemInventory;
    [Space]
    [Header("Enemy Part Variables")]
    [SerializeField] public bool isWeak;
    [SerializeField] public bool isDetachable;
    [SerializeField] public int hitPoints;
    //[Space]
    //[Header("Read Only Variables"), ReadOnly]
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {

    }
    public void TakeDamage(float damage, WeaponData weaponData)
    {
        if (isWeak)
        {
            damage *= weaponData.bulletCritMultiplier;
        }
        if (enemyScript.health > 0) //handles damage on life
        {
            enemyScript.health -= damage;
            if (enemyScript.health <= 0)
            {
                enemyScript.OnDeath();
            }
            //PopupDmg(damage);
            enemyScript.timeSinceLastSeen = 0;
        }
        else if(isDetachable && spriteItemInventory!= null && prefabItemInventory != null) //handles chopping, makes sure variables are corretly setted 
        {
            if (hitPoints > 0)
            {
                hitPoints-=(int)damage;
            }
            else 
            {
                prefabDroppedItem.GetComponent<ItemSpawner>().SpawnItem(transform.position, spriteItemInventory);
                
                gameObject.SetActive(false);
            }
        }
        foreach (Rigidbody rb in enemyScript.activateRagdoll.rigidbodies)
        {
            //Apply force
            Vector3 forceDirection = (rb.gameObject.transform.position - enemyScript.player.transform.position).normalized;
            rb.AddForce(weaponData.explosionForce * forceDirection, ForceMode.Impulse);
        }

        
    }
}
