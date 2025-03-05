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
                GameObject item = Instantiate(prefabDroppedItem, transform.position,Quaternion.identity);
                
                if (item.TryGetComponent<Rigidbody>(out Rigidbody rb)) //applies explosion force on spawn
                {
                    // Calcula la dirección opuesta al jugador
                    Vector3 forceDirection = (rb.gameObject.transform.position - enemyScript.player.transform.position).normalized;

                    // Añade una componente hacia arriba a la dirección
                    Vector3 launchDirection = (forceDirection + Vector3.up).normalized;

                    // Aplica la fuerza con un factor multiplicador para ajustar la magnitud
                    rb.AddForce(1.5f * launchDirection, ForceMode.Impulse);
                }
                if (item.transform.GetChild(0).TryGetComponent<Animator>(out Animator animator)) //Sets animation active
                {
                    animator.SetLayerWeight(1, 1); //Changes weight
                }
                if (item.transform.GetChild(0).GetChild(0).TryGetComponent<SpriteRenderer>(out SpriteRenderer renderer)) //Try gets image component else will have default image
                {
                    renderer.sprite = spriteItemInventory;
                }
                
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
