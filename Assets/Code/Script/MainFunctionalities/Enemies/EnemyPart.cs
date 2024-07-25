using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPart : MonoBehaviour, IDamageable
{
    public Enemy enemyScript;
    public bool isCritical;
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
        if (isCritical)
        {
            enemyScript.gotCritHit = true;
            enemyScript.TakeDamage(damage * weaponData.bulletCritMultiplier, weaponData);            
        }
        else { enemyScript.TakeDamage(damage, weaponData); }
    }
}
