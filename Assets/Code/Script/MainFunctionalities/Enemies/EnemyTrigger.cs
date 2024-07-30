using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    [SerializeField] public bool triggeringAttack;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            triggeringAttack = true;

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggeringAttack = false;

        }
    }
}
