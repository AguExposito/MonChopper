using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public float health = 100f;
    ActivateRagdoll activateRagdoll;
    // Start is called before the first frame update
    void Start()
    {
        activateRagdoll = GetComponent<ActivateRagdoll>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(float damage)
    {
        if (health > 0)
        {
            health -= damage;
            if (health <= 0)
            {
                activateRagdoll.SetEnabled(true);
            }
        }
    }
}
