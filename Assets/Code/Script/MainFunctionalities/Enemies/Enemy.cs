using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD.MinMaxSlider;
using Unity.Collections;
using TMPro;
using System.Reflection;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private GameObject popupDmg;
    [SerializeField] private LayerMask obstacleMask;
    private GameObject player;

    [Header("Variables")]
    [SerializeField] private Color dmgColor;
    [SerializeField] private Color weakSpotCcolor;
    [SerializeField] public float health = 100f;
    [SerializeField] private Vector2Int minMaxXp;
    [SerializeField] public int giveXp;
    [SerializeField] float attackDmg;
    [SerializeField] float attackRate;
    [SerializeField] float fovAngle;
    [SerializeField] float viewDistance;
    [SerializeField] float viewRadius;
    [SerializeField] float followTime;

    [Header("Text Variables")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float randomDirForce;
    [SerializeField] private float timeToDisable;

    [Header("Read Only Variables"), ReadOnly]
    [SerializeField] private NavMeshAgent enemyNMAgent;
    [SerializeField] private Collider attack;
    [SerializeField] private Collider attackTrigger;
    [SerializeField] private Animator enemyAnimator;
    [SerializeField] float timeSinceLastAttack;
    [SerializeField] float timeSinceLastSeen;
    [SerializeField] private GameObject dmgTxtContainer;
    [SerializeField] private GameObject popupCanvas;
    [SerializeField] public bool isDead = false;
    [SerializeField] public bool isAttacking = false;
    [SerializeField] public bool gotWeakSpotHit = false;
    [SerializeField] ActivateRagdoll activateRagdoll;
    // Start is called before the first frame update
    void Start()
    {
        activateRagdoll = GetComponent<ActivateRagdoll>();
        player = GameObject.FindWithTag("Player");
        giveXp = Random.Range(minMaxXp.x, minMaxXp.y);

        popupCanvas = gameObject.transform.Find("PopupsCanvas").gameObject;
        dmgTxtContainer = popupCanvas.transform.GetChild(0).gameObject;

        enemyNMAgent = GetComponent<NavMeshAgent>();
        enemyAnimator = GetComponent<Animator>();
        attack = transform.Find("Attack").GetComponent<Collider>();
        attackTrigger = attack.transform.Find("Trigger").GetComponent<Collider>();

        timeSinceLastSeen = followTime;
    }

    // Update is called once per frame
    void Update()
    {
        //Counts how much time will enemy follow up player
        if (timeSinceLastSeen < followTime)
        {
            timeSinceLastSeen += Time.deltaTime;
        }
        //Resets counter
        if (IsPlayerInVision())
        {
            timeSinceLastSeen = 0;
        }
        //Manages if nav mesh should activate or not
        if (!isDead && (IsPlayerInVision() || timeSinceLastSeen<followTime))
        {
            enemyNMAgent.SetDestination(player.transform.position);
            if (enemyNMAgent.remainingDistance <= enemyNMAgent.stoppingDistance+0.2f) //Si la distancia es <= a la distancia en la que se tendría que detener + offset, --> mira al player
            {
                var lookPos = player.transform.position - transform.position;
                lookPos.y = 0;
                var rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2);
            }
        }

        //popupCanvas always looks at player
        popupCanvas.transform.LookAt(transform.position + player.transform.rotation * Vector3.forward, player.transform.rotation * Vector3.up);

        //Attack Methods
        Attack();
        
    }
    bool IsPlayerInVision()
    {
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        // Verifica si el jugador está dentro de la distancia de visión
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        // Verifica si el jugador está dentro del ángulo de visión
        if (angleToPlayer < fovAngle / 2)
        {            
            if (distanceToPlayer < viewDistance)
            {
                // Realiza un raycast para verificar si hay una línea de visión clara
                if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleMask))
                {
                    return true;
                }
            }
        }
        if (distanceToPlayer < viewRadius) {
            if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleMask))
            {
                return true;
            }
        }

        return false;
    }
    public void TakeDamage(float damage, WeaponData weaponData)
    {
        if (health > 0)
        {
            health -= damage;
            if (health <= 0)
            {
                OnDeath(weaponData);
            }
            //PopupDmg(damage);
            timeSinceLastSeen = 0;
        }
        foreach (Rigidbody rb in activateRagdoll.rigidbodies)
        {
            //Apply force
            Vector3 forceDirection = (rb.gameObject.transform.position - player.transform.position).normalized;
            rb.AddForce(weaponData.explosionForce * forceDirection, ForceMode.Impulse);
        }
    }
    void OnDeath(WeaponData weaponData) {
        isDead = true;

        //Activate ragdoll
        activateRagdoll.SetEnabled(true);

        //Disable NavMesh
        transform.GetComponent<NavMeshAgent>().enabled=false;

        //give xp
        player.GetComponent<Player>().GetXp(giveXp);

        //deletes all dmgTxt and canvas
        StartCoroutine(DestroyCanvas());        
    }

    #region PopupCanvasDMG
    void PopupDmg(float dmg) {
        GameObject dmgTxt=null;
        
        if (dmgTxtContainer.transform.childCount == 0)
        {
            dmgTxt = Instantiate(popupDmg, dmgTxtContainer.transform);
        }
        else {
            for (int i = 0; i < dmgTxtContainer.transform.childCount; i++)
            {
                if (!dmgTxtContainer.transform.GetChild(i).gameObject.activeInHierarchy)
                {
                    dmgTxt = dmgTxtContainer.transform.GetChild(i).gameObject;
                    dmgTxt.SetActive(true);
                    break;
                }
                else {
                    if (i== dmgTxtContainer.transform.childCount-1) {
                        dmgTxt = Instantiate(popupDmg, dmgTxtContainer.transform);
                        break;
                    }
                }
            }
        }
        ChangeDmgTxtVariables(dmgTxt);

        dmgTxt.GetComponent<TextMeshProUGUI>().text = dmg.ToString();

        //Reset rigidbody variables
        dmgTxt.transform.localPosition = Vector3.zero;
        dmgTxt.GetComponent<Rigidbody>().velocity = Vector3.zero;

        //dmgTxt JumpForce
        dmgTxt.GetComponent<Rigidbody>().AddForce(Vector3.up*jumpForce, ForceMode.Impulse);

        //dmgTxt randomDirection
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        dmgTxt.GetComponent<Rigidbody>().AddForce(randomDirection * randomDirForce, ForceMode.Impulse);
        StartCoroutine(DisableDmgTxt(dmgTxt));
    }

    IEnumerator DisableDmgTxt(GameObject dmgTxt) {
        yield return new WaitForSeconds(timeToDisable);
        dmgTxt.SetActive(false);
    }

    IEnumerator DestroyCanvas()
    {
        yield return new WaitForSeconds(timeToDisable);
        Destroy(dmgTxtContainer.gameObject);
    }
    void ChangeDmgTxtVariables(GameObject dmgTxt) {

        dmgTxt.GetComponent<TextMeshProUGUI>().color = gotWeakSpotHit ? weakSpotCcolor : dmgColor;
        dmgTxt.GetComponent<TextMeshProUGUI>().fontSize = gotWeakSpotHit ? 0.5f: 0.25f;
        gotWeakSpotHit = false;
    }

    #endregion

    #region Attack
    public void ActivateAttakVFX()
    {
        for (int i = 0; i < attack.transform.childCount; i++)
        {
            if (attack.transform.GetChild(i).CompareTag("AttackVFX"))
            {
                attack.transform.GetChild(i).GetComponent<ParticleSystem>().Play();
                break;
            }
        }
    }

    void Attack()
    {
        if (attackTrigger.GetComponent<EnemyTrigger>().triggeringAttack && !isAttacking && timeSinceLastAttack >= 1f / attackRate && !isDead && IsPlayerInVision()) {
            enemyAnimator.SetTrigger("Attack");
            isAttacking = true;
            timeSinceLastAttack = 0;
            //Apply Damage to Player
            if (attack.bounds.Intersects(player.GetComponent<CharacterController>().bounds))
            {
                IDamageable damageable = player.GetComponent<IDamageable>();
                damageable?.TakeDamage(attackDmg, null);
            }
        }
        if (!isAttacking) //Delay Attak
        {
            if (timeSinceLastAttack < 1f / attackRate)
            {
                timeSinceLastAttack += Time.deltaTime;
            }
        }
    }

    public void ToogleAttackState(int state) //0=false, other number = true
    {
        if (state == 0)
        {
            isAttacking = false;
        }
        else { isAttacking = true; }
    }
    #endregion
    private void OnDrawGizmos()
    {
        // Color de las líneas del cono de visión
        Gizmos.color = Color.yellow;

        // Dibuja el cono de visión
        Vector3 forward = transform.forward * viewDistance;
        Vector3 rightBoundary = Quaternion.Euler(0, fovAngle / 2, 0) * forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -fovAngle / 2, 0) * forward;

        // Dibuja los límites del cono de visión
        Gizmos.DrawRay(transform.position, rightBoundary);
        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawWireSphere(transform.position,viewRadius);

        // Dibuja una esfera en la distancia máxima de visión para dar contexto
        Gizmos.DrawWireSphere(transform.position + forward, 0.5f);
    }
}
