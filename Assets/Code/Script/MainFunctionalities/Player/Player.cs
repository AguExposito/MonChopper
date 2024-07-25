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
    public int xp;
    public float level;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TakeDamage(float damage, WeaponData weaponData) // player takes damage
    {
        if (armor > damage)
        {
            armor -= damage;
        }
        else { 
            armor = 0;
            health -= damage-armor;
        }
        transform.Find("CanvasHUD").GetComponent<HudController>().UpdateHudValues(); //update values in hud
    }
    public void GetXp(int givenXp) {
        xp += givenXp;
        Debug.Log("Got xp: " + givenXp);
    }
}
