using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFireball : StateMachineBehaviour
{
    public float throwForce;

    public Transform bossHand;
    public Transform player;
    public GameObject fireballPrefab;
    private GameObject fireball;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        fireball = Instantiate(fireballPrefab, bossHand);
    }

    public void ThrowFireball()
    {
        if (fireball == null)
            return;

        fireball.transform.parent = null;
        Rigidbody rb = fireball.GetComponent<Rigidbody>();
        Vector3 force = (fireball.transform.position - player.transform.position).normalized * throwForce;
        rb.AddForce(force, ForceMode.Impulse);
        fireball.transform.rotation = Quaternion.LookRotation(rb.velocity);

        if (fireball != null)
            Destroy(fireball, 8f);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
