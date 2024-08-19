using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Collections;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Weapon weapon;

    [Header("Player Variables")]
    [SerializeField] TextMeshProUGUI healthTxt;
    [SerializeField] TextMeshProUGUI armorTxt;
    [SerializeField] TextMeshProUGUI xpTxt;
    [SerializeField] TextMeshProUGUI levelTxt;
    [SerializeField] TextMeshProUGUI ammoTxt;

    [Header("Read Only Variables"), ReadOnly]
    [SerializeField] Player player;    
    // Start is called before the first frame update
    void Start()
    {
        player = transform.parent.GetComponent<Player>();
        if (weapon.transform.childCount!=0) {
            UpdateHudValues();
        }
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    public void UpdateHudValues()
    {
        healthTxt.text = player.health + "/" + player.healthMax;
        armorTxt.text = player.armor + "/" + player.armorMax;
        xpTxt.text = player.xp + "/" + player.xpMax;
        levelTxt.text = player.level.ToString();

        healthTxt.transform.parent.GetComponent<UnityEngine.UI.Image>().fillAmount = player.health / player.healthMax;
        armorTxt.transform.parent.GetComponent<UnityEngine.UI.Image>().fillAmount = player.armor / player.armorMax;
        levelTxt.transform.parent.GetComponent<UnityEngine.UI.Image>().fillAmount = player.xp / player.xpMax;
        if (weapon.weaponData != null)
        {
            ammoTxt.text = weapon.weaponData.currentAmmo + "/" + weapon.weaponData.ammoAmount;
        }
    }
}
