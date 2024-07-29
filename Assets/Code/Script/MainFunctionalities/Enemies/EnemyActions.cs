using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyActions : MonoBehaviour
{
    NavMeshAgent enemyNMAgent;
    Enemy enemy;
    Player player;
    // Start is called before the first frame update
    void Start()
    {
        enemyNMAgent = GetComponent<NavMeshAgent>();
        enemy = transform.GetComponent<Enemy>();
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!enemy.isDead)
        {
            enemyNMAgent.SetDestination(player.transform.position);
        }
    }
}

