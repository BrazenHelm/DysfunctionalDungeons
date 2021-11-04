using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseAnimation : MonoBehaviour, IPausable
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    public void Pause()
    {
        animator.enabled = false;
    }


    public void Unpause()
    {
        animator.enabled = true;
    }
}
