using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class PopupDMG : MonoBehaviour
{
    [Header("References")]    
    [SerializeField] private GameObject popupDmg;

    [Space]
    [Header("Variables")]
    [SerializeField] private Color dmgColor;
    [SerializeField] private Color weakSpotCcolor;
    [SerializeField] private float jumpForce;
    [SerializeField] private float randomDirForce;
    [SerializeField] private float timeToDisable;

    [Space]
    [Header("Read Only Variables"), Unity.Collections.ReadOnly]
    [SerializeField] private GameObject player;
    [SerializeField] public bool gotWeakSpotHit = false;
    [SerializeField] private GameObject dmgTxtContainer;
    // Start is called before the first frame update
    void Start()
    {
        player = transform.parent.parent.gameObject;
        dmgTxtContainer = GameObject.FindWithTag("dmgTxtContainer");
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    #region PopupCanvasDMG
    public void PopupDmg(float dmg, Vector3 position)
    {
        GameObject dmgTxt = null;

        if (dmgTxtContainer.transform.childCount == 0)
        {
            dmgTxt = Instantiate(popupDmg, dmgTxtContainer.transform);
            dmgTxt.GetComponent<RectTransform>().position = position;
        }
        else
        {
            for (int i = 0; i < dmgTxtContainer.transform.childCount; i++)
            {
                if (!dmgTxtContainer.transform.GetChild(i).gameObject.activeInHierarchy)
                {
                    dmgTxt = dmgTxtContainer.transform.GetChild(i).gameObject;
                    dmgTxt.SetActive(true);
                    dmgTxt.GetComponent<RectTransform>().position = position;
                    break;
                }
                else
                {
                    if (i == dmgTxtContainer.transform.childCount - 1)
                    {
                        dmgTxt = Instantiate(popupDmg, dmgTxtContainer.transform);
                        dmgTxt.GetComponent<RectTransform>().position = position;
                        break;
                    }
                }
            }
        }
        //dmgTxt.transform.LookAt(position + player.transform.rotation * Vector3.forward, player.transform.rotation * Vector3.up);
        ChangeDmgTxtVariables(dmgTxt);

        dmgTxt.GetComponent<TextMeshProUGUI>().text = dmg.ToString();

        //Reset rigidbody variables
        //dmgTxt.transform.localPosition = Vector3.zero;
        dmgTxt.GetComponent<Rigidbody>().velocity = Vector3.zero;

        //dmgTxt JumpForce
        dmgTxt.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        //dmgTxt randomDirection
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        dmgTxt.GetComponent<Rigidbody>().AddForce(randomDirection * randomDirForce, ForceMode.Impulse);
        StartCoroutine(DisableDmgTxt(dmgTxt));
    }

    IEnumerator DisableDmgTxt(GameObject dmgTxt)
    {
        float i=0;
        while (i<timeToDisable) { 
            dmgTxt.transform.LookAt(dmgTxt.transform.position + player.transform.rotation * Vector3.forward, player.transform.rotation * Vector3.up);
            i += Time.deltaTime;
            yield return null;
        }
        dmgTxt.SetActive(false);
    }

    void ChangeDmgTxtVariables(GameObject dmgTxt)
    {

        dmgTxt.GetComponent<TextMeshProUGUI>().color = gotWeakSpotHit ? weakSpotCcolor : dmgColor;
        dmgTxt.GetComponent<TextMeshProUGUI>().fontSize = gotWeakSpotHit ? 0.5f : 0.25f;
        gotWeakSpotHit = false;
    }

    #endregion

}
