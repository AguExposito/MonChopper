using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD.MinMaxSlider;
using Unity.Collections;
using TMPro;
using System.Reflection;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject popupDmg;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Variables")]
    [SerializeField] public float health = 100f;
    [SerializeField] private Vector2Int minMaxXp;
    [SerializeField] public int giveXp;
    [SerializeField] float attackDmg;
    [SerializeField] float attackRate;
    [SerializeField] float fovAngle;
    [SerializeField] float viewDistance;
    [SerializeField] float viewRadius;
    [SerializeField] float followTime;

    [Header("Read Only Variables"), ReadOnly]
    [SerializeField] public GameObject player;
    [SerializeField] private NavMeshAgent enemyNMAgent;
    [SerializeField] private Collider attack;
    [SerializeField] private Collider attackTrigger;
    [SerializeField] private Animator enemyAnimator;
    [SerializeField] private float timeSinceLastAttack;
    [SerializeField] public float timeSinceLastSeen;
    [SerializeField] public bool isDead = false;
    [SerializeField] public bool isAttacking = false;
    [SerializeField] public ActivateRagdoll activateRagdoll;
    // Start is called before the first frame update
    void Start()
    {
        activateRagdoll = GetComponent<ActivateRagdoll>();
        player = GameObject.FindWithTag("Player");
        giveXp = Random.Range(minMaxXp.x, minMaxXp.y);

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
        else if (enemyNMAgent.isActiveAndEnabled && enemyNMAgent.remainingDistance <= enemyNMAgent.stoppingDistance + 0.2f)  //if reached destination and not following player
        {
            enemyNMAgent.SetDestination(transform.position); //Sets destination (itself)
            enemyAnimator.SetBool("Walk", false);
        }
        //Resets counter
        if (IsPlayerInVision())
        {
            timeSinceLastSeen = 0;
        }
        //Manages if nav mesh should activate or not
        if (!isDead && (IsPlayerInVision() || timeSinceLastSeen<followTime))
        {
            enemyAnimator.SetBool("Walk",true);
            enemyNMAgent.SetDestination(player.transform.position); //Sets destination (player)
            if (enemyNMAgent.remainingDistance <= enemyNMAgent.stoppingDistance+0.2f) //Si la distancia es <= a la distancia en la que se tendría que detener + offset, --> mira al player
            {
                var lookPos = player.transform.position - transform.position;
                lookPos.y = 0;
                var rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2);
            }
        }
        //Attack Methods
        Attack();
        
    }
    bool IsPlayerInVision()
    {
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        // Verifies if Player is inside vision distance
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        // Verifies if Player is inside vision angle
        if (angleToPlayer < fovAngle / 2)
        {            
            if (distanceToPlayer < viewDistance)
            {
                // Perform a raycast to check if there is a clear line of sight
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
    public void OnDeath() {
        isDead = true;

        //Activate ragdoll
        activateRagdoll.SetEnabled(true);

        //Disable NavMesh
        transform.GetComponent<NavMeshAgent>().enabled=false;

        //give xp
        player.GetComponent<Player>().GetXp(giveXp);  
        
        //Disables Attack visuals
        attack.gameObject.SetActive(false);
    }

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
        // Color of vision cone
        Gizmos.color = Color.yellow;

        // Draw the vision cone
        Vector3 forward = transform.forward * viewDistance;
        Vector3 rightBoundary = Quaternion.Euler(0, fovAngle / 2, 0) * forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -fovAngle / 2, 0) * forward;

        // Draw the boundaries of the vision cone
        Gizmos.DrawRay(transform.position, rightBoundary);
        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawWireSphere(transform.position,viewRadius);

        // Draw a sphere at maximum viewing distance to give context
        Gizmos.DrawWireSphere(transform.position + forward, 0.5f);
    }
}
