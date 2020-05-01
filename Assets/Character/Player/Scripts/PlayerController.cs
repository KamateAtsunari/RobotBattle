//using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //[SerializeField] 
    //private GameObject particle = null;
    private Animator animator;
    private bool interval;
    // Use this for initialization
    void Start()
    {
        //target = GameObject.Find("hannin").transform;
        //particleManager = particle.GetComponent<ParticleManager>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!interval)
        {
            if (Input.GetKey("q"))
            {
                animator.SetTrigger("SAttack");

            }
            else if (Input.GetKeyUp("q"))
            {
                animator.SetFloat("SAttackSpeed", 1);
                //particleManager.ChangeActive(false);
                animator.ResetTrigger("SAttack");
            }
            if (Input.GetKeyDown("e"))
            {
                animator.SetTrigger("Kick");
            }

            if (Input.GetMouseButtonDown(0))
            {
                animator.SetTrigger("Attack");
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            animator.SetBool("isGuard", true);
        }
        if (Input.GetMouseButtonUp(1))
        {
            animator.SetBool("isGuard", false);
        }
    }
    public void ChangeIntebal(bool rigid)
    {
        interval = rigid;
    }
}
