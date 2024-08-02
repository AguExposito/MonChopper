using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPart : MonoBehaviour, IDamageable
{
    public Enemy enemyScript;
    public bool isWeak;
    // Start is called before the first frame update
    void Start()
    {
        
    }

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
        if (enemyScript.health > 0)
        {
            enemyScript.health -= damage;
            if (enemyScript.health <= 0)
            {
                enemyScript.OnDeath(weaponData);
            }
            //PopupDmg(damage);
            enemyScript.timeSinceLastSeen = 0;
        }
        foreach (Rigidbody rb in enemyScript.activateRagdoll.rigidbodies)
        {
            //Apply force
            Vector3 forceDirection = (rb.gameObject.transform.position - enemyScript.player.transform.position).normalized;
            rb.AddForce(weaponData.explosionForce * forceDirection, ForceMode.Impulse);
        }

        
    }
}
