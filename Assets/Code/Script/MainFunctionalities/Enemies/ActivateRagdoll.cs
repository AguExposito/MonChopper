using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActivateRagdoll : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] List<Rigidbody> rigidbodies;

    void Start()
    {
        rigidbodies = transform.GetComponentsInChildren<Rigidbody>().ToList();
        SetEnabled(false);
    }

    public void SetEnabled(bool enabled)
    {
        bool isKinematic = !enabled;
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = isKinematic;
        }

        animator.enabled = !enabled;
    }
}

