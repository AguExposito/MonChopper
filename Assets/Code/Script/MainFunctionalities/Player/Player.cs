using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    [Header("Player Variables")]
    public float healthMax;
    public float armorMax;
    public float xpMax;

    [Space]
    public float health;
    public float armor;
    public float xp;
    public float level;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TakeDamage(float damage)
    {
        
    }
}
